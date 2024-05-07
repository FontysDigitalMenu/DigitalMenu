using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mollie.Api.Client;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Response;
using Serilog;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/split")]
[ApiController]
public class SplitController(
    ISplitService splitService,
    IOrderService orderService,
    ICartItemService cartItemService,
    IHubContext<OrderHub, IOrderHubClient> hubContext) : ControllerBase
{
    [HttpPost("pay")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> Pay([FromBody] PaySplitRequest request)
    {
        Split? split = splitService.GetById(request.SplitId);
        if (split == null)
        {
            return NotFound(new { Message = "Split not found" });
        }

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await splitService.CreateMolliePayment(split);
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

        return Ok(new SplitPayedViewModel
        {
            RedirectUrl = paymentResponse.Links.Checkout.Href,
            OrderId = split.OrderId,
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

        Split? split = splitService.GetByExternalPaymentId(request.id);
        if (split == null)
        {
            return Ok();
        }

        PaymentResponse paymentResponse;
        try
        {
            paymentResponse = await splitService.GetPaymentFromMollie(request.id);
        }
        catch (MollieApiException e)
        {
            return BadRequest(new { e.Message });
        }

        split.PaymentStatus = paymentResponse.Status switch
        {
            PaymentStatus.Paid => DigitalMenu_20_BLL.Enums.PaymentStatus.Paid,
            PaymentStatus.Canceled => DigitalMenu_20_BLL.Enums.PaymentStatus.Canceled,
            PaymentStatus.Expired => DigitalMenu_20_BLL.Enums.PaymentStatus.Expired,
            var _ => DigitalMenu_20_BLL.Enums.PaymentStatus.Pending,
        };
        if (!splitService.Update(split))
        {
            return BadRequest(new { Message = "Split could not be updated" });
        }

        if (split.Order.Splits.All(s => s.PaymentStatus == DigitalMenu_20_BLL.Enums.PaymentStatus.Paid))
        {
            orderService.ProcessPaidOrder(split.Order);
            await SendOrderInfoToKitchenAndCustomers(split.Order);
        }

        return Ok();
    }

    private async Task SendOrderInfoToKitchenAndCustomers(Order order)
    {
        Order? orderWithNewSplitData = orderService.GetBy(order.Id);
        if (orderWithNewSplitData == null)
        {
            return;
        }

        OrderViewModel orderViewModel = OrderViewModel.FromOrder(orderWithNewSplitData, cartItemService);
        await hubContext.Clients.All.ReceiveOrder(orderViewModel);
        await hubContext.Clients.Group($"order-{orderWithNewSplitData.Id}").ReceiveOrderUpdate(orderViewModel);
    }
}