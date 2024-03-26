using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IConfiguration configuration, IOrderService orderService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Post([FromBody] OrderRequest orderRequest)
    {
        string? apiKey = configuration.GetValue<string>("Mollie:ApiKey");
        string? redirectUrl = configuration.GetValue<string>("Mollie:RedirectUrl");
        if (apiKey == null || redirectUrl == null)
        {
            return Problem();
        }

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
            paymentResponse = await orderService.CreateMolliePayment(apiKey, redirectUrl, totalAmount, orderId);
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

        return CreatedAtAction("Get", new { id = order.Id }, new OrderCreatedViewModel
        {
            RedirectUrl = paymentResponse.Links.Checkout.Href,
            OrderId = order.Id,
        });
    }

    [HttpGet("{id}/{deviceId}/{tableId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get([FromRoute] string id, [FromRoute] string deviceId, [FromRoute] string tableId)
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

        string? apiKey = configuration.GetValue<string>("Mollie:ApiKey");
        if (apiKey == null)
        {
            return Problem();
        }

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await orderService.GetPaymentFromMollie(apiKey, order.ExternalPaymentId);
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

        return Ok(new OrderViewModel
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
            }).ToList(),
        });
    }
}