using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/cartItem")]
[ApiController]
public class CartItemController(
    ICartItemService cartItemService,
    IMenuItemService menuItemService,
    ITableService tableService,
    IIngredientService ingredientService,
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

                CartViewModel reuslt = GetCartViewModel(cartRequest.TableSessionId);
                await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}").ReceiveCartUpdate(reuslt);

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

        CartViewModel reuslt2 = GetCartViewModel(cartRequest.TableSessionId);

        await hubContext.Clients.Group($"cart-{cartRequest.TableSessionId}").ReceiveCartUpdate(reuslt2);

        return NoContent();
    }

    [HttpGet]
    public IActionResult GetCartItem(int cartItemId, string tableSessionId)
    {
        if (tableService.GetBySessionId(tableSessionId) == null)
        {
            return NotFound();
        }

        CartItem? cartItem = cartItemService.GetByCartItemIdAndTableSessionId(cartItemId, tableSessionId);

        if (cartItem == null)
        {
            return NotFound();
        }

        MenuItem? menuitem = menuItemService.GetMenuItemById(cartItem.MenuItemId);
        if (menuitem == null)
        {
            return NotFound();
        }

        cartItem.MenuItem = menuitem;

        List<Ingredient> excludedIngredients = cartItemService.GetExcludedIngredientsByCartItemId(cartItemId);

        CartItemWithIngredientsViewModel cartItemWithIngredients = new()
        {
            CartItem = cartItem,
            ExcludedIngredients = excludedIngredients,
        };

        return Ok(cartItemWithIngredients);
    }

    [HttpGet("{tableSessionId}")]
    public IActionResult ViewCart([FromRoute] string tableSessionId)
    {
        if (tableService.GetBySessionId(tableSessionId) == null)
        {
            return NotFound();
        }

        bool cartItemsExists = cartItemService.ExistsByTableSessionId(tableSessionId);
        if (!cartItemsExists)
        {
            return NotFound();
        }

        return Ok(GetCartViewModel(tableSessionId));
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
            .ReceiveCartUpdate(GetCartViewModel(cartRequest.TableSessionId));

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
            .ReceiveCartUpdate(GetCartViewModel(cartRequest.TableSessionId));

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
            .ReceiveCartUpdate(GetCartViewModel(cartRequest.TableSessionId));

        return NoContent();
    }

    private CartViewModel GetCartViewModel(string tableSessionId)
    {
        List<CartItem> cartItems = cartItemService.GetByTableSessionId(tableSessionId);

        return new CartViewModel
        {
            CartItems = cartItems.Select(cartItem => new CartItemViewModel
            {
                Id = cartItem.Id,
                Note = cartItem.Note,
                Quantity = cartItem.Quantity,
                TableSessionId = cartItem.TableSessionId,
                MenuItemId = cartItem.MenuItemId,
                MenuItem = new MenuItemViewModel
                {
                    Id = cartItem.MenuItem.Id,
                    Name = cartItem.MenuItem.Name,
                    Price = cartItem.MenuItem.Price,
                    ImageUrl = cartItem.MenuItem.ImageUrl,
                },
            }).ToList(),
            TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity),
        };
    }
}