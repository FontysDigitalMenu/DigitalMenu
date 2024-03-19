using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers
{
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
			var cartItems = _cartItemService.GetAll();
			var existingCartItem = cartItems.FirstOrDefault(item => item.Id == id);

			if (existingCartItem != null)
			{
				existingCartItem.Quantity++;
			}
			else
			{
				var menuItem = _menuItemService.GetMenuItemBy(id);

				if (menuItem != null)
				{
					var newCartItem = new CartItem
					{
						Id = id,
						MenuItem = menuItem,
						Quantity = 1,
						Note = note
					};

					_cartItemService.Create(newCartItem);
				}
			}

			return Ok();
		}

		[HttpGet]
		public IActionResult ViewCart()
		{
			var cartItems = _cartItemService.GetAll();

			var cartViewModel = new CartItemViewModel
			{
				CartItems = cartItems,
				TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity)
			};

			return Ok(cartViewModel);
		}

		[HttpDelete]
		public IActionResult RemoveFromCart(int id)
		{
			var cartItems = _cartItemService.GetAll();
			var itemToRemove = cartItems.FirstOrDefault(item => item.Id == id);

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
}
