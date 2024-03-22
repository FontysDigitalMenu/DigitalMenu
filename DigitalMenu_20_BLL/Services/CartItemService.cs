using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CartItemService : ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;

    public CartItemService(ICartItemRepository cartItemRepository)
    {
        _cartItemRepository = cartItemRepository;
    }

    public List<CartItem> GetAll()
    {
        return _cartItemRepository.GetAll();
    }

    public CartItem GetById(int id)
    {
        return _cartItemRepository.GetById(id);
    }

    public void Create(CartItem cartItem)
    {
        _cartItemRepository.Create(cartItem);
    }
}