using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_30_DAL.Repositories
{
	public class CartItemRepository : ICartItemRepository
	{
		private readonly ApplicationDbContext _dbContext;

		public CartItemRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<CartItem> GetAll()
		{
			return _dbContext.CartItems.ToList();
		}

		public CartItem GetById(int id)
		{
			return _dbContext.CartItems.Find(id);
		}

		public void Create(CartItem cartItem)
		{
			_dbContext.CartItems.Add(cartItem);
		}
	}
}
