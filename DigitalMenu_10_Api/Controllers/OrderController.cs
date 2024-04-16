﻿using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mollie.Api.Client;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Response;
using Serilog;

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

        return Ok(orders.Select(o => new OrderViewModel
        {
            Id = o.Id,
            PaymentStatus = o.PaymentStatus.ToString(),
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            OrderDate = o.OrderDate,
            OrderNumber = o.OrderNumber,
            MenuItems = o.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
        }));
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

        return Ok(new OrderViewModel
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
        });
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

        await SendOrderToKitchen(order);

        return Ok();
    }

    private async Task SendOrderToKitchen(Order order)
    {
        OrderViewModel orderViewModel = new()
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
        };

        await hubContext.Clients.All.ReceiveOrder(orderViewModel);
    }

    [Authorize(Roles = "Admin, Employee")]
    [HttpGet("paid")]
    public ActionResult<List<OrderViewModel>> GetPaidOrders()
    {
        List<Order> orders = (List<Order>)orderService.GetPaidOrders();

        List<OrderViewModel> orderViewModels = orders.Select(order => new OrderViewModel
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
        }).ToList();

        return Ok(orderViewModels);
    }
}