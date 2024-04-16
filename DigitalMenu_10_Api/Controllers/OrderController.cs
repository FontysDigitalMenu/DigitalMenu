using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Enums;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mollie.Api.Client;
using Mollie.Api.Models.Payment.Response;
using Serilog;
using PaymentStatus = Mollie.Api.Models.Payment.PaymentStatus;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/order")]
[ApiController]
public class OrderController(
    IOrderService orderService,
    IHubContext<OrderHub, IOrderHubClient> hubContext,
    ICartItemService cartItemService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<OrderCreatedViewModel>> Post([FromBody] OrderRequest orderRequest)
    {
        int totalAmount;
        try
        {
            totalAmount = orderService.GetTotalAmount(orderRequest.DeviceId, orderRequest.TableId);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }

        string orderId = Guid.NewGuid().ToString();

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await orderService.CreateMolliePayment(totalAmount, orderId);
        }
        catch (MollieApiException e)
        {
            if (e.Details.Status == 401)
            {
                return Unauthorized(new { Message = "Unauthorized Mollie API-Key" });
            }

            return BadRequest(new { Message = "Mollie error" });
        }
        catch (Exception)
        {
            return BadRequest(new { Message = "Payment by Mollie could not be created" });
        }

        Order order;
        try
        {
            order = orderService.Create(orderRequest.DeviceId, orderRequest.TableId, paymentResponse.Id, orderId);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (DatabaseCreationException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (DatabaseUpdateException e)
        {
            return BadRequest(new { e.Message });
        }

        return CreatedAtAction("Get", new { id = order.Id, deviceId = orderRequest.DeviceId, orderRequest.TableId },
            new OrderCreatedViewModel
            {
                RedirectUrl = paymentResponse.Links.Checkout.Href,
                OrderId = order.Id,
            });
    }

    [HttpGet("{deviceId}/{tableId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<List<OrderViewModel>> Get([FromRoute] string deviceId, [FromRoute] string tableId)
    {
        List<Order>? orders;
        try
        {
            orders = orderService.GetBy(deviceId, tableId);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }

        if (orders == null || orders.Count == 0)
        {
            return NotFound(new { Message = "Order not found" });
        }

        return Ok(orders.Select(o => OrderViewModel.FromOrder(o, cartItemService)));
    }

    [HttpGet("{id}/{deviceId}/{tableId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<OrderViewModel> Get([FromRoute] string id, [FromRoute] string deviceId,
        [FromRoute] string tableId)
    {
        Order? order;
        try
        {
            order = orderService.GetBy(id, deviceId, tableId);
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }

        if (order == null)
        {
            return NotFound(new { Message = "Order not found" });
        }

        return Ok(OrderViewModel.FromOrder(order, cartItemService));
    }

    [DisableCors]
    [HttpPost("webhook")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Webhook([FromForm] WebhookRequest request)
    {
        Log.Information("Webhook received {@request}", request);

        Order? order = orderService.GetByExternalPaymentId(request.id);
        if (order == null)
        {
            return Ok();
        }

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await orderService.GetPaymentFromMollie(request.id);
        }
        catch (MollieApiException e)
        {
            return BadRequest(new { e.Message });
        }

        order.PaymentStatus = paymentResponse.Status switch
        {
            PaymentStatus.Paid => DigitalMenu_20_BLL.Enums.PaymentStatus.Paid,
            PaymentStatus.Canceled => DigitalMenu_20_BLL.Enums.PaymentStatus.Canceled,
            PaymentStatus.Expired => DigitalMenu_20_BLL.Enums.PaymentStatus.Expired,
            var _ => DigitalMenu_20_BLL.Enums.PaymentStatus.Pending,
        };
        if (!orderService.Update(order))
        {
            return BadRequest(new { Message = "Order could not be updated" });
        }

        if (order.PaymentStatus == DigitalMenu_20_BLL.Enums.PaymentStatus.Paid)
        {
            orderService.ProcessPaidOrder(order);
            await SendOrderToKitchen(order);
        }

        return Ok();
    }

    [HttpGet("testWebsocket/{id}")]
    public async Task<IActionResult> TestWebsocket([FromRoute] string id)
    {
        Order? order = orderService.GetByExternalPaymentId(id);
        if (order == null)
        {
            return NotFound();
        }

        orderService.ProcessPaidOrder(order);
        await SendOrderToKitchen(order);

        return Ok();
    }

    private async Task SendOrderToKitchen(Order order)
    {
        OrderViewModel orderViewModel = OrderViewModel.FromOrder(order, cartItemService);
        await hubContext.Clients.All.ReceiveOrder(orderViewModel);
    }

    [Authorize(Roles = "Admin, Employee")]
    [HttpGet("paid/{type}")]
    public ActionResult<List<OrderViewModel>> GetPaidOrders(string type)
    {
        IEnumerable<Order> orders;

        switch (type)
        {
            case "food":
                orders = orderService.GetPaidFoodOrders();
                break;
            case "drinks":
                orders = orderService.GetPaidDrinksOrders();
                break;
            default:
                orders = orderService.GetPaidOrders();
                break;
        }

        List<OrderViewModel> orderViewModels =
            orders.Select(o => OrderViewModel.FromOrder(o, cartItemService)).ToList();

        return Ok(orderViewModels);
    }

    [Authorize(Roles = "Admin, Employee")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [HttpPut("{id}")]
    public ActionResult Put([FromRoute] string id, [FromBody] OrderUpdateRequest orderRequest)
    {
        Order? order = orderService.GetBy(id);
        if (order == null)
        {
            return NotFound(new { Message = "Order not found" });
        }

        switch (orderRequest.OrderStatus)
        {
            case "Pending":
                order.Status = OrderStatus.Pending;
                break;
            case "Processing":
                order.Status = OrderStatus.Processing;
                break;
            case "Done":
                order.Status = OrderStatus.Done;
                break;
            case "Completed":
                order.Status = OrderStatus.Completed;
                break;
            default:
                return BadRequest(new { Message = "Invalid OrderStatus" });
        }

        if (!orderService.Update(order))
        {
            return BadRequest(new { Message = "Order could not be updated" });
        }

        OrderViewModel orderViewModel = OrderViewModel.FromOrder(order, cartItemService);
        hubContext.Clients.Group($"order-{order.Id}").ReceiveOrderUpdate(orderViewModel);

        return NoContent();
    }
}