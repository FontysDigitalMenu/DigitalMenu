using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IConfiguration configuration, ApplicationDbContext dbContext, IOrderService orderService)
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
        string? redirectUrl = configuration.GetValue<string>("FrontendUrl");
        if (apiKey == null)
        {
            return Problem("Mollie API key is not set");
        }

        if (redirectUrl == null)
        {
            return Problem("Redirect url is not set");
        }

        int totalAmount;
        try
        {
            totalAmount = orderService.GetTotalAmount(orderRequest.DeviceId, orderRequest.TableId);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await orderService.CreateMolliePayment(apiKey, redirectUrl, totalAmount);
        }
        catch (Exception)
        {
            return BadRequest("Payment by Mollie could not be created");
        }

        Order order;
        try
        {
            order = orderService.Create(orderRequest.DeviceId, orderRequest.TableId, paymentResponse.Id);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (DatabaseCreationException e)
        {
            return BadRequest(e.Message);
        }
        catch (DatabaseUpdateException e)
        {
            return BadRequest(e.Message);
        }

        return CreatedAtAction("Get", new { id = order.Id }, new OrderCreatedViewModel
        {
            RedirectUrl = paymentResponse.Links.Checkout.Href,
            OrderId = order.Id,
        });
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        Order? order = orderService.GetById(id);
        if (order == null)
        {
            return NotFound("Order not found");
        }

        string? apiKey = configuration.GetValue<string>("Mollie:ApiKey");
        if (apiKey == null)
        {
            return Problem("Mollie API key is not set");
        }

        using IPaymentClient paymentClient = new PaymentClient(apiKey);
        PaymentResponse? result;
        try
        {
            result = await paymentClient.GetPaymentAsync(order.ExternalPaymentId);
            if (result == null)
            {
                return Problem();
            }
        }
        catch (MollieApiException e)
        {
            if (e.Details.Status == 404)
            {
                return NotFound("Payment not found");
            }

            return Problem();
        }

        order.PaymentStatus = result.Status switch
        {
            PaymentStatus.Paid => DigitalMenu_20_BLL.Enums.PaymentStatus.Paid,
            PaymentStatus.Canceled => DigitalMenu_20_BLL.Enums.PaymentStatus.Canceled,
            PaymentStatus.Expired => DigitalMenu_20_BLL.Enums.PaymentStatus.Expired,
            var _ => DigitalMenu_20_BLL.Enums.PaymentStatus.Pending,
        };
        orderService.Update(order);

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

    [HttpGet("paid")]
    public IActionResult GetPaidOrders()
    {
        List<Order> orders = (List<Order>)orderService.GetPaidOrders();

        List<OrderViewModel> orderViewModels = orders.Select(order => new OrderViewModel
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
        }).ToList();

        return Ok(orderViewModels);
    }
}