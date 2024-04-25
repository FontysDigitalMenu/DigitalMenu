using System.ComponentModel.DataAnnotations;
using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Enums;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/order")]
[ApiController]
public class OrderController(
    IOrderService orderService,
    ICartItemService cartItemService,
    IHubContext<OrderHub, IOrderHubClient> hubContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public ActionResult<SplitPayedViewModel> Post([FromBody] OrderRequest orderRequest)
    {
        List<Split> splits = orderRequest.Splits.Select(s => new Split
        {
            Name = s.Name,
            Amount = s.Amount,
        }).ToList();

        Order createdOrder;
        try
        {
            createdOrder = orderService.Create(orderRequest.DeviceId, orderRequest.TableSessionId, splits);
        }
        catch (ValidationException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (NotFoundException e)
        {
            return NotFound(new { e.Message });
        }
        catch (DatabaseCreationException e)
        {
            return BadRequest(new { e.Message });
        }

        return CreatedAtAction("Get",
            new { id = createdOrder.Id, deviceId = createdOrder.DeviceId, tableId = createdOrder.TableId },
            OrderViewModel.FromOrder(createdOrder, cartItemService));
    }

    [HttpGet("{tableId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<List<OrderViewModel>> Get([FromRoute] string tableId)
    {
        List<Order>? orders;
        try
        {
            orders = orderService.GetByTableId(tableId);
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

    [HttpGet("{id}/{tableSessionId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<OrderViewModel> Get([FromRoute] string id, [FromRoute] string tableSessionId)
    {
        Order? order;
        try
        {
            order = orderService.GetBy(id, tableSessionId);
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
                if (orderRequest.IsDrinks)
                    order.DrinkStatus = OrderStatus.Pending;
                else
                    order.FoodStatus = OrderStatus.Pending;
                break;
            case "Processing":
                if (orderRequest.IsDrinks)
                    order.DrinkStatus = OrderStatus.Processing;
                else
                    order.FoodStatus = OrderStatus.Processing;
                break;
            case "Done":
                if (orderRequest.IsDrinks)
                    order.DrinkStatus = OrderStatus.Done;
                else
                    order.FoodStatus = OrderStatus.Done;
                break;
            case "Completed":
                if (orderRequest.IsDrinks)
                    order.DrinkStatus = OrderStatus.Completed;
                else
                    order.FoodStatus = OrderStatus.Completed;
                break;
            default:
                return BadRequest(new { Message = "Invalid OrderStatus" });
        }

        if (!orderService.Update(order))
        {
            return BadRequest(new { Message = "Order could not be updated" });
        }

        OrderViewModel orderViewModel = OrderViewModel.FromOrder(order, cartItemService);

        if (!orderRequest.IsDrinks)
        {
            hubContext.Clients.Group($"order-{order.Id}").ReceiveOrderUpdate(orderViewModel);
        }

        return NoContent();
    }
}