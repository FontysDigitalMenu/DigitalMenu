using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using System.Text.Json;
using DigitalMenu_10_Api.JsonObjects;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IConfiguration configuration, ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Pay()
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

        Order order = new()
        {
            DeviceId = "12345",
            TableId = "1",
            Note = "Test order",
            TotalAmount = 100,
            Quantity = 1,
            Table = dbContext.Tables.First(),
        };
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        using IPaymentClient paymentClient = new PaymentClient($"{apiKey}", new HttpClient());
        PaymentRequest paymentRequest = new()
        {
            Amount = new Amount(Currency.EUR, 100.00m),
            Description = "Test payment of the example project",
            RedirectUrl = redirectUrl,
            Method = Mollie.Api.Models.Payment.PaymentMethod.Ideal,
        };
        paymentRequest.SetMetadata(new
        {
            OrderId = order.Id,
        });

        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
        string checkoutUrl = paymentResponse.Links.Checkout.Href;

        return Ok(checkoutUrl);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        string? apiKey = configuration.GetValue<string>("Mollie:ApiKey");
        if (apiKey == null)
        {
            return BadRequest("Redirect url is not set");
        }

        using IPaymentClient paymentClient = new PaymentClient(apiKey);
        
        PaymentResponse? result;
        try
        {
            result = await paymentClient.GetPaymentAsync(id);
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

        if (result.Metadata == null)
        {
            return NotFound("Order not found");
        }
        int orderId = JsonSerializer.Deserialize<OrderJson>(result.Metadata)!.OrderId;
        
        Order? order = await dbContext.Orders.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(new OrderViewModel
        {
            PaymentStatus = result.Status,
            Order = order,
        });
    }
}