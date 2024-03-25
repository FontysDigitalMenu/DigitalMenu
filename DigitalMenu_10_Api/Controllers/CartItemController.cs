using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CartItemController : ControllerBase
{
    private readonly ICartItemService _cartItemService;

    private readonly IMenuItemService _menuItemService;
    private readonly IIngredientService _ingredientService;

    public CartItemController(ICartItemService cartItemService, IMenuItemService menuItemService, IIngredientService ingredientService)
    {
        _cartItemService = cartItemService;
        _menuItemService = menuItemService;
        _ingredientService = ingredientService;
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartRequest cartRequest)
    {
        CartItem? cartItem = _cartItemService.GetByMenuItemIdAndDeviceId(cartRequest.MenuItemId, cartRequest.DeviceId);

        if (cartItem != null)
        {
            cartItem.Quantity++;
            //TODO: Overwrite or append note?
            // cartItem.Note = cartRequest.Note;

            _cartItemService.Update(cartItem);
        }
        else
        {
            if (_menuItemService.GetMenuItemById(cartRequest.MenuItemId) == null)
            {
                return NotFound();
            }

            CartItem newCartItem = new()
            {
                Note = cartRequest.Note,
                Quantity = 1,
                DeviceId = cartRequest.DeviceId,
                MenuItemId = cartRequest.MenuItemId,
            };

            _cartItemService.Create(newCartItem);

            if (cartRequest.ExcludedIngredients != null && cartRequest.ExcludedIngredients.Count != 0)
            {
                foreach (string excludedIngredientName in cartRequest.ExcludedIngredients)
                {
                    Ingredient? excludedIngredient = await _ingredientService.GetIngredientByNameAsync(excludedIngredientName);
                    
                    if (excludedIngredient != null)
                    {
                        ExcludedIngredientCartItem excludedIngredientCartItem = new()
                        {
                            IngredientId = excludedIngredient.Id,
                            CartItemId = newCartItem.Id
                        };

                        _cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
                    }
                }
            }
        }

        return NoContent();
    }

    [HttpGet("{id:int}")]
    public IActionResult GetCartItem(int id)
    {
        // TODO: make it so the cart item gets selected instead of a random menu item
        MenuItem? menuitem = _menuItemService.GetMenuItemById(id);
        if (menuitem == null)
        {
            return NotFound();
        }

        List<Ingredient> excludedIngredients = _cartItemService.GetExcludedIngredientsByCartItemId(id);

        CartItemWithIngredientsViewModel cartItem = new()
        {
            MenuItem = menuitem,
            ExcludedIngredients = excludedIngredients
        };

        return Ok(cartItem);
    }

    [HttpGet("{deviceId}")]
    public IActionResult ViewCart([FromRoute] string deviceId)
    {
        bool cartItemsExists = _cartItemService.ExistsByDeviceId(deviceId);
        if (!cartItemsExists)
        {
            return NotFound();
        }

        List<CartItem> cartItems = _cartItemService.GetByDeviceId(deviceId);

        CartItemViewModel cartViewModel = new()
        {
            CartItems = cartItems,
            TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity),
        };

        return Ok(cartViewModel);
    }

    [HttpPut]
    public IActionResult RemoveFromCart([FromBody] CartUpdateRequest cartRequest)
    {
        CartItem? cartItem = _cartItemService.GetByMenuItemIdAndDeviceId(cartRequest.MenuItemId, cartRequest.DeviceId);

        if (cartItem == null)
        {
            return NotFound();
        }

        if (cartItem.Quantity > 1)
        {
            cartItem.Quantity--;

            _cartItemService.Update(cartItem);
        }
        else
        {
            _cartItemService.Delete(cartItem);
        }

        return NoContent();
    }
}