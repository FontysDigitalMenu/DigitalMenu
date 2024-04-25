using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/cartItem")]
[ApiController]
public class CartItemController(
    ICartItemService cartItemService,
    IMenuItemService menuItemService,
    IIngredientService ingredientService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartRequest cartRequest)
    {
        List<CartItem?> cartItems =
            cartItemService.GetCartItemsByMenuItemIdAndTableSessionId(cartRequest.MenuItemId, cartRequest.TableSessionId);

        foreach (CartItem? cartItem in cartItems)
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

        return NoContent();
    }

    [HttpGet]
    public IActionResult GetCartItem(int cartItemId, string tableSessionId)
    {
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
        bool cartItemsExists = cartItemService.ExistsByTableSessionId(tableSessionId);
        if (!cartItemsExists)
        {
            return NotFound();
        }

        List<CartItem> cartItems = cartItemService.GetByTableSessionId(tableSessionId);

        CartItemViewModel cartViewModel = new()
        {
            CartItems = cartItems,
            TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity),
        };

        return Ok(cartViewModel);
    }

    [HttpPut("minus")]
    public IActionResult Minus1FromCart([FromBody] CartUpdateRequest cartRequest)
    {
        CartItem? cartItem = cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

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

        return NoContent();
    }

    [HttpPost("plus")]
    public IActionResult Plus1ToCart([FromBody] CartUpdateRequest cartRequest)
    {
        CartItem? cartItem = cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

        if (cartItem == null)
        {
            return NotFound();
        }

        cartItem.Quantity++;
        cartItemService.Update(cartItem);

        return NoContent();
    }

    [HttpPut("updateDetails")]
    public async Task<ActionResult> UpdateCartItem([FromBody] CartItemRequest cartRequest)
    {
        CartItem? cartItem = cartItemService.GetByCartItemIdAndTableSessionId(cartRequest.CartItemId, cartRequest.TableSessionId);

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

        return NoContent();
    }
}