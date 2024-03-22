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
    public IActionResult AddToCart(int id, string note)
    {
        List<CartItem> cartItems = _cartItemService.GetAll();
        CartItem? existingCartItem = cartItems.FirstOrDefault(item => item.Id == id);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity++;
        }
        else
        {
            MenuItem? menuItem = _menuItemService.GetMenuItemBy(id);

            if (menuItem != null)
            {
                CartItem newCartItem = new CartItem
                {
                    Id = id,
                    MenuItem = menuItem,
                    Quantity = 1,
                    Note = note,
                };

                _cartItemService.Create(newCartItem);
            }
        }

        return Ok();
    }

    [HttpGet]
    public IActionResult ViewCart()
    {
        List<CartItem> cartItems = _cartItemService.GetAll();

        CartItemViewModel cartViewModel = new CartItemViewModel
        {
            CartItems = cartItems,
            TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity),
        };

        return Ok(cartViewModel);
    }

    [HttpDelete]
    public IActionResult RemoveFromCart(int id)
    {
        List<CartItem> cartItems = _cartItemService.GetAll();
        CartItem? itemToRemove = cartItems.FirstOrDefault(item => item.Id == id);

        if (itemToRemove != null)
        {
            if (itemToRemove.Quantity > 1)
            {
                itemToRemove.Quantity--;
            }
            else
            {
                cartItems.Remove(itemToRemove);
            }
        }

        return Ok();
    }
}