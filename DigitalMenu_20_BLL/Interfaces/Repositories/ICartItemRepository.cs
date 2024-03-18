using DigitalMenu_20_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Repositories
{
	public interface ICartItemRepository
	{
		public CartItem GetById(int id);
		public void Create(CartItem cartItem);
	}
}
