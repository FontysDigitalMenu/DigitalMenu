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

    private readonly IIngredientService _ingredientService;

    private readonly IMenuItemService _menuItemService;

    public CartItemController(ICartItemService cartItemService, IMenuItemService menuItemService,
        IIngredientService ingredientService)
    {
        _cartItemService = cartItemService;
        _menuItemService = menuItemService;
        _ingredientService = ingredientService;
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartRequest cartRequest)
    {
        List<CartItem?> cartItems =
            _cartItemService.GetCartItemsByMenuItemIdAndDeviceId(cartRequest.MenuItemId, cartRequest.DeviceId);

        foreach (CartItem? cartItem in cartItems)
        {
            List<Ingredient> existingExcludedIngredients =
                _cartItemService.GetExcludedIngredientsByCartItemId(cartItem.Id);
            List<string> existingExcludedIngredientNames = existingExcludedIngredients.Select(e => e.Name).ToList();
            bool sameExcludedIngredients = existingExcludedIngredientNames.OrderBy(n => n)
                .SequenceEqual(cartRequest.ExcludedIngredients.OrderBy(n => n));

            if (sameExcludedIngredients && cartItem.Note == cartRequest.Note)
            {
                cartItem.Quantity++;
                _cartItemService.Update(cartItem);

                return NoContent();
            }
        }

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
                Ingredient? excludedIngredient =
                    await _ingredientService.GetIngredientByNameAsync(excludedIngredientName);

                if (excludedIngredient != null)
                {
                    ExcludedIngredientCartItem excludedIngredientCartItem = new()
                    {
                        IngredientId = excludedIngredient.Id,
                        CartItemId = newCartItem.Id,
                    };

                    _cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
                }
            }
        }

        return NoContent();
    }

    [HttpGet]
    public IActionResult GetCartItem(int cartItemId, string deviceId)
    {
        CartItem? cartItem = _cartItemService.GetByCartItemIdAndDeviceId(cartItemId, deviceId);

        if (cartItem == null)
        {
            return NotFound();
        }

        MenuItem? menuitem = _menuItemService.GetMenuItemById(cartItem.MenuItemId);
        if (menuitem == null)
        {
            return NotFound();
        }

        cartItem.MenuItem = menuitem;

        List<Ingredient> excludedIngredients = _cartItemService.GetExcludedIngredientsByCartItemId(cartItemId);

        CartItemWithIngredientsViewModel cartItemWithIngredients = new()
        {
            CartItem = cartItem,
            ExcludedIngredients = excludedIngredients,
        };

        return Ok(cartItemWithIngredients);
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

    [HttpPut("minus")]
    public IActionResult Minus1FromCart([FromBody] CartUpdateRequest cartRequest)
    {
        CartItem? cartItem = _cartItemService.GetByCartItemIdAndDeviceId(cartRequest.CartItemId, cartRequest.DeviceId);

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

    [HttpPost("plus")]
    public IActionResult Plus1ToCart([FromBody] CartUpdateRequest cartRequest)
    {
        CartItem? cartItem = _cartItemService.GetByCartItemIdAndDeviceId(cartRequest.CartItemId, cartRequest.DeviceId);

        if (cartItem == null)
        {
            return NotFound();
        }

        cartItem.Quantity++;
        _cartItemService.Update(cartItem);

        return NoContent();
    }

    [HttpPut("updateDetails")]
    public async Task<ActionResult> UpdateCartItem([FromBody] CartItemRequest cartRequest)
    {
        CartItem? cartItem = _cartItemService.GetByCartItemIdAndDeviceId(cartRequest.CartItemId, cartRequest.DeviceId);

        if (cartItem == null)
        {
            NotFound();
        }
        else
        {
            cartItem.Note = cartRequest.Note;
            _cartItemService.Update(cartItem);
            _cartItemService.DeleteExcludedIngredientsFromCartItem(cartItem.Id);

            if (cartRequest.ExcludedIngredients != null && cartRequest.ExcludedIngredients.Count != 0)
            {
                foreach (string excludedIngredientName in cartRequest.ExcludedIngredients)
                {
                    Ingredient? excludedIngredient =
                        await _ingredientService.GetIngredientByNameAsync(excludedIngredientName);

                    if (excludedIngredient != null)
                    {
                        ExcludedIngredientCartItem excludedIngredientCartItem = new()
                        {
                            IngredientId = excludedIngredient.Id,
                            CartItemId = cartItem.Id,
                        };

                        _cartItemService.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
                    }
                }
            }
        }

        return NoContent();
    }
}