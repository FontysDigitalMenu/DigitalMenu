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
		private readonly List<CartItem> _cartItems;

		public CartItemController(ICartItemService cartItemService, IMenuItemService menuItemService)
		{
			_cartItemService = cartItemService;
			_menuItemService = menuItemService;
			_cartItems = [];
		}

		[HttpPost]
		public IActionResult AddToCart(int id, string note)
		{
			var itemToAdd = _cartItemService.GetById(id);
			var existingCartItem = _cartItems.FirstOrDefault(item => item.Id == id);

			if (existingCartItem != null)
			{
				existingCartItem.Quantity++;
			}
			else
			{
				// var menuItem = _menuItemService.GetById(id);
				// if (menuItem != null)
				{
					var newCartItem = new CartItem
					{
						Id = id,
						Quantity = 1,
						Note = note
					};

					_cartItems.Add(newCartItem);
				}
			}

			return Ok();
		}

		[HttpGet]
		public IActionResult ViewCart()
		{
			var cartViewModel = new CartItemViewModel
			{
				CartItems = _cartItems,
				TotalAmount = _cartItems.Sum(item => item.MenuItem.Price * item.Quantity)
			};

			return Ok();
		}

		[HttpDelete]
		public IActionResult RemoveFromCart(int id)
		{
			var itemToRemove = _cartItems.FirstOrDefault(item => item.Id == id);

			if (itemToRemove != null)
			{
				if (itemToRemove.Quantity > 1)
				{
					itemToRemove.Quantity--;
				}
				else
				{
					_cartItems.Remove(itemToRemove);
				}
			}

			return Ok();
		}
	}
}
