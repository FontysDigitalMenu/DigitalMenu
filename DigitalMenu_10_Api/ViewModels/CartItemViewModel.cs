using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels
{
	public class CartItemViewModel
	{
		public List<CartItem> CartItems { get; set; }

		public int TotalAmount { get; set; }
	}
}
