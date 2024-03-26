using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IConfiguration configuration, ApplicationDbContext dbContext, IOrderService orderService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] OrderRequest orderRequest)
    {
        string? apiKey = configuration.GetValue<string>("Mollie:ApiKey");
        string? redirectUrl = configuration.GetValue<string>("FrontendUrl");
        if (apiKey == null)
        {
            return BadRequest("Redirect url is not set");
        }

        if (redirectUrl == null)
        {
            return BadRequest("Mollie API key is not set");
        }

        int totalAmount = orderService.GetTotalAmount();

        using IPaymentClient paymentClient = new PaymentClient($"{apiKey}", new HttpClient());
        PaymentRequest paymentRequest = new()
        {
            Amount = new Amount(Currency.EUR, (decimal)totalAmount / 100),
            Description = "Order payment",
            RedirectUrl = redirectUrl,
            Method = PaymentMethod.Ideal,
        };
        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);

        Order? order =
            orderService.Create(orderRequest.DeviceId, orderRequest.TableId, paymentResponse.Id, totalAmount);
        if (order == null)
        {
            return BadRequest("Order could not be created");
        }

        OrderCreatedViewModel orderCreatedViewModel = new()
        {
            RedirectUrl = paymentResponse.Links.Checkout.Href,
            OrderId = order.Id,
        };

        return CreatedAtAction("Get", new { id = order.Id }, orderCreatedViewModel);
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
            return BadRequest("Redirect url is not set");
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