using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ICartItemRepository
{
    public List<CartItem> GetAll();

    public CartItem GetById(int id);

    public void Create(CartItem cartItem);
}