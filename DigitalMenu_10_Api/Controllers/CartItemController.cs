using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartItemController : ControllerBase
{
    private readonly ICartItemService _cartItemService;

    private readonly IMenuItemService _menuItemService;

    public CartItemController(ICartItemService cartItemService, IMenuItemService menuItemService)
    {
        _cartItemService = cartItemService;
        _menuItemService = menuItemService;
    }

    [HttpPost]
    public IActionResult AddToCart(CartRequest cartRequest)
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
                Id = cartRequest.MenuItemId,
                Note = cartRequest.Note,
                Quantity = 1,
                DeviceId = cartRequest.DeviceId,
                MenuItemId = cartRequest.MenuItemId,
            };

            _cartItemService.Create(newCartItem);
        }

        return NoContent();
    }

    [HttpGet]
    public IActionResult ViewCart(string deviceId)
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
    public IActionResult RemoveFromCart(CartUpdateRequest cartRequest)
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