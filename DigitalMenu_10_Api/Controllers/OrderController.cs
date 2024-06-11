using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.Services;
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
    IReservationService reservationService,
    ICartItemService cartItemService,
    IIngredientService ingredientService,
    IHubContext<OrderHub, IOrderHubClient> hubContext,
    IServiceProvider serviceProvider) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<SplitPayedViewModel>> Post([FromBody] OrderRequest orderRequest)
    {
        List<Split> splits = orderRequest.Splits.Select(s => new Split
        {
            Name = s.Name,
            Amount = s.Amount,
        }).ToList();

        Order createdOrder;
        try
        {
            createdOrder = await orderService.Create(orderRequest.TableSessionId, splits);
        }
        catch (Exception e)
        {
            return BadRequest(new { e.Message });
        }
        /*catch (ValidationException e)
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
        }*/

        cartItemService.ClearByTableSessionId(orderRequest.TableSessionId);

        IHubContext<IngredientHub, IIngredientHubClient> ingredientHubContext =
            serviceProvider.GetRequiredService<IHubContext<IngredientHub, IIngredientHubClient>>();

        List<Ingredient> ingredients = await ingredientService.GetIngredients();

        List<IngredientViewModel> ingredientViewModels = ingredients.Select(ingredient => new IngredientViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Stock = ingredient.Stock,
        }).ToList();

        await ingredientHubContext.Clients.All.ReceiveIngredient(ingredientViewModels);

        await hubContext.Clients.Group($"cart-{orderRequest.TableSessionId}")
            .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                orderRequest.TableSessionId));

        return CreatedAtAction("Get",
            new { id = createdOrder.Id, tableSessionId = createdOrder.SessionId },
            OrderViewModel.FromOrder(createdOrder, cartItemService));
    }

    [HttpGet("{tableSessionId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<List<OrderViewModel>> Get([FromRoute] string tableSessionId)
    {
        List<Order>? orders;
        try
        {
            orders = orderService.GetByTableSessionId(tableSessionId);
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

    [Authorize(Roles = "Admin, Employee")]
    [HttpGet("completed/{type}")]
    public ActionResult<List<OrderViewModel>> GetCompletedOrders([FromRoute] string type)
    {
        IEnumerable<Order> orders;

        switch (type)
        {
            case "food":
                orders = orderService.GetCompletedFoodOrders();
                break;
            case "drinks":
                orders = orderService.GetCompletedDrinksOrders();
                break;
            default:
                orders = orderService.GetCompletedOrders();
                break;
        }

        List<OrderViewModel> orderViewModels =
            orders.Select(o => OrderViewModel.FromOrder(o, cartItemService)).ToList();

        return Ok(orderViewModels);
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

        OrderViewModel orderViewModel = OrderViewModel.FromOrderWithCatagory(order, cartItemService);

        List<MenuItemViewModel> drinkMenuItemViewModels = orderViewModel.MenuItems
            .Where(mi => mi.Categories!.Contains("Drinks"))
            .ToList();

        List<MenuItemViewModel> foodMenuItemViewModels = orderViewModel.MenuItems
            .Where(mi => !mi.Categories!.Contains("Drinks"))
            .ToList();

        if (drinkMenuItemViewModels.Count != 0)
        {
            orderViewModel.HasDrinks = true;
        }

        if (foodMenuItemViewModels.Count != 0)
        {
            orderViewModel.HasFood = true;
        }

        return Ok(orderViewModel);
    }

    [Authorize(Roles = "Admin, Employee")]
    [HttpGet("paid/{type}")]
    public ActionResult<List<OrderViewModel>> GetPaidOrders([FromRoute] string type)
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

        OrderViewModel orderViewModel = OrderViewModel.FromOrderWithCatagory(order, cartItemService);

        List<MenuItemViewModel> drinkMenuItemViewModels = orderViewModel.MenuItems
            .Where(mi => mi.Categories!.Contains("Drinks"))
            .ToList();

        List<MenuItemViewModel> foodMenuItemViewModels = orderViewModel.MenuItems
            .Where(mi => !mi.Categories!.Contains("Drinks"))
            .ToList();

        if (drinkMenuItemViewModels.Count != 0)
        {
            orderViewModel.HasDrinks = true;
        }

        if (foodMenuItemViewModels.Count != 0)
        {
            orderViewModel.HasFood = true;
        }

        if (!orderService.Update(order))
        {
            return BadRequest(new { Message = "Order could not be updated" });
        }

        hubContext.Clients.Group($"order-{order.Id}").ReceiveOrderUpdate(orderViewModel);

        hubContext.Clients.All.ReceiveOrderDrinksUpdate();

        return NoContent();
    }
}