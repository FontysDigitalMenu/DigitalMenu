using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.Services;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/cartItem")]
[ApiController]
public class CartItemController(
    ICartItemService cartItemService,
    IMenuItemService menuItemService,
    ITableService tableService,
    IIngredientService ingredientService,
    IOrderService orderService,
    IReservationService reservationService,
    IHubContext<OrderHub, IOrderHubClient> hubContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartRequest cartRequest)
    {
        if (tableService.GetBySessionId(cartRequest.TableSessionId) == null)
        {
            return NotFound();
        }

        List<CartItem> cartItems =
            cartItemService.GetCartItemsByMenuItemIdAndTableSessionId(cartRequest.MenuItemId,
                cartRequest.TableSessionId);

        foreach (CartItem cartItem in cartItems)
        {
            List<Ingredient> existingExcludedIngredients =
                cartItemService.GetExcludedIngredientsByCartItemId(cartItem.Id);
            List<string> existingExcludedIngredientNames = existingExcludedIngredients.Select(e => e.Name).ToList();
            bool sameExcludedIngredients = existingExcludedIngredientNames.OrderBy(n => n)
                .SequenceEqual(cartRequest.ExcludedIngredients.OrderBy(n => n));

            if (sameExcludedIngredients && cartItem.Note == cartRequest.Note)
            {
                cartItem.Quantity++;
                cartItemService.Update(cartItem);

                await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}")
                    .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                        cartRequest.TableSessionId));

                return NoContent();
            }
        }

        if (menuItemService.GetMenuItemById(cartRequest.MenuItemId) == null)
        {
            return NotFound();
        }

        CartItem newCartItem = new()
        {
            Note = cartRequest.Note,
            Quantity = 1,
            TableSessionId = cartRequest.TableSessionId,
            MenuItemId = cartRequest.MenuItemId,
        };

        cartItemService.Create(newCartItem);

        if (cartRequest.ExcludedIngredients != null && cartRequest.ExcludedIngredients.Count != 0)
        {
            foreach (string excludedIngredientName in cartRequest.ExcludedIngredients)
            {
                Ingredient? excludedIngredient =
                    await ingredientService.GetIngredientByNameAsync(excludedIngredientName);

                if (excludedIngredient != null)
                {
                    ExcludedIngredientCartItem excludedIngredientCartItem = new()
                    {
                        IngredientId = excludedIngredient.Id,
                        CartItemId = newCartItem.Id,
                    };

                    cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
                }
            }
        }

        await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}")
            .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                cartRequest.TableSessionId));

        return NoContent();
    }

    [HttpGet]
    public IActionResult GetCartItem(int cartItemId, string tableSessionId)
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        if (tableService.GetBySessionId(tableSessionId) == null)
        {
            return NotFound();
        }

        CartItem? cartItem = cartItemService.GetByCartItemIdAndTableSessionId(cartItemId, tableSessionId);

        if (cartItem == null)
        {
            return NotFound();
        }

        CartItemViewModel cartItemViewModel = new()
        {
            Id = cartItem.Id,
            Note = cartItem.Note,
            Quantity = cartItem.Quantity,
            TableSessionId = cartItem.TableSessionId,
            MenuItemId = cartItem.MenuItemId,
        };

        MenuItem? menuitem = menuItemService.GetMenuItemById(cartItemViewModel.MenuItemId);
        if (menuitem == null)
        {
            return NotFound();
        }

        MenuItemViewModel menuItemViewModel = new()
        {
            Id = menuitem.Id,
            Name = menuitem.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Name ?? menuitem.Name,
            Description = menuitem.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Description ??
                          menuitem.Description,
            Price = menuitem.Price,
            ImageUrl = menuitem.ImageUrl,
            Note = cartItem.Note,
            Ingredients = menuitem.Ingredients.Select(i => new IngredientViewModel
            {
                Id = i.Id,
                Name = i.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Name ?? i.Name,
                Stock = i.Stock,
                Pieces = i.Pieces,
            }).ToList(),
            IsActive = menuitem.IsActive,
            Categories = menuitem.Categories.Select(c => c.Name).ToList(),
        };

        cartItemViewModel.MenuItem = menuItemViewModel;

        List<Ingredient> excludedIngredients = cartItemService.GetExcludedIngredientsByCartItemId(cartItemId);

        List<IngredientViewModel> excludedIngredientsViewModel = excludedIngredients.Select(e => new IngredientViewModel
        {
            Id = e.Id,
            Name = e.Name,
            Stock = e.Stock,
            Pieces = e.Pieces,
        }).ToList();

        CartItemWithIngredientsViewModel cartItemWithIngredients = new()
        {
            CartItem = cartItemViewModel,
            ExcludedIngredients = excludedIngredientsViewModel,
        };

        return Ok(cartItemWithIngredients);
    }

    [HttpGet("{tableSessionId}")]
    public IActionResult ViewCart([FromRoute] string tableSessionId)
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        if (tableService.GetBySessionId(tableSessionId) == null)
        {
            return NotFound();
        }

        return Ok(CartService.GetCartViewModel(reservationService, orderService, cartItemService, tableSessionId,
            localeValue));
    }

    [HttpPut("minus")]
    public async Task<IActionResult> Minus1FromCart([FromBody] CartUpdateRequest cartRequest)
    {
        if (tableService.GetBySessionId(cartRequest.TableSessionId) == null)
        {
            return NotFound();
        }

        CartItem? cartItem =
            cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

        if (cartItem == null)
        {
            return NotFound();
        }

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;

            cartItemService.Update(cartItem);
        }
        else
        {
            cartItemService.Delete(cartItem);
        }

        await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}")
            .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                cartRequest.TableSessionId));

        return NoContent();
    }

    [HttpPost("plus")]
    public async Task<IActionResult> Plus1ToCart([FromBody] CartUpdateRequest cartRequest)
    {
        if (tableService.GetBySessionId(cartRequest.TableSessionId) == null)
        {
            return NotFound();
        }

        CartItem? cartItem =
            cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

        if (cartItem == null)
        {
            return NotFound();
        }

        cartItem.Quantity++;
        cartItemService.Update(cartItem);

        await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}")
            .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                cartRequest.TableSessionId));

        return NoContent();
    }

    [HttpPut("updateDetails")]
    public async Task<ActionResult> UpdateCartItem([FromBody] CartItemRequest cartRequest)
    {
        if (tableService.GetBySessionId(cartRequest.TableSessionId) == null)
        {
            return NotFound();
        }

        CartItem? cartItem =
            cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

        if (cartItem == null)
        {
            NotFound();
        }
        else
        {
            cartItem.Note = cartRequest.Note;
            cartItemService.Update(cartItem);
            cartItemService.DeleteExcludedIngredientsFromCartItem(cartItem.Id);

            if (cartRequest.ExcludedIngredients != null && cartRequest.ExcludedIngredients.Count != 0)
            {
                foreach (string excludedIngredientName in cartRequest.ExcludedIngredients)
                {
                    Ingredient? excludedIngredient =
                        await ingredientService.GetIngredientByNameAsync(excludedIngredientName);

                    if (excludedIngredient != null)
                    {
                        ExcludedIngredientCartItem excludedIngredientCartItem = new()
                        {
                            IngredientId = excludedIngredient.Id,
                            CartItemId = cartItem.Id,
                        };

                        cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
                    }
                }
            }
        }

        await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}")
            .ReceiveCartUpdate(CartService.GetCartViewModel(reservationService, orderService, cartItemService,
                cartRequest.TableSessionId));

        return NoContent();
    }
}