using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_20_BLL.Interfaces;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrderController(IConfiguration configuration, ApplicationDbContext dbContext, IOrderService orderService) : ControllerBase
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
            Method = Mollie.Api.Models.Payment.PaymentMethod.Ideal,
        };
        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);
        
        bool orderCreated = orderService.Create(orderRequest.DeviceId, orderRequest.TableId, paymentResponse.Id, totalAmount);
        if (!orderCreated)
        {
            return BadRequest("Order could not be created");
        }
        
        return Ok(paymentResponse.Links.Checkout.Href);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        Order? order = await dbContext.Orders.FindAsync(id);
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
            PaymentStatus = result.Status,
            Order = order,
        });
    }
}