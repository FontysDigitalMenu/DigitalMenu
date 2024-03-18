using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Models
{
	public class CartItem
	{
		public int Id { get; set; }

		public string Note { get; set; }

		public int Quantity { get; set; }

		public MenuItem MenuItem { get; set; }
	}
}
