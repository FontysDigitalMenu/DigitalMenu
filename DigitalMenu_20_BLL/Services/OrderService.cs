using System.ComponentModel.DataAnnotations;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using shortid;
using shortid.Configuration;

namespace DigitalMenu_20_BLL.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartItemRepository cartItemRepository,
    ITableRepository tableRepository,
    ISplitRepository splitRepository) : IOrderService
{
    public Order Create(string deviceId, string tableId, List<Split> splits)
    {
        if (!cartItemRepository.ExistsByTableSessionId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        Table? table = tableRepository.GetById(tableId);
        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByTableSessionId(deviceId);
        if (cartItems.Count == 0)
        {
            throw new NotFoundException("CartItems do not exist");
        }

        List<OrderMenuItem> orderMenuItems = cartItems.Select(ci => new OrderMenuItem
        {
            MenuItemId = ci.MenuItemId,
            MenuItem = ci.MenuItem,
            Quantity = ci.Quantity,
            Note = ci.Note,
            ExcludedIngredientOrderMenuItems = cartItemRepository.GetExcludedIngredientsByCartItemId(ci.Id).Select(ei =>
                new ExcludedIngredientOrderMenuItem
                {
                    IngredientId = ei.Id,
                }).ToList(),
        }).ToList();

        int totalAmount = GetTotalAmount(deviceId, tableId);
        if (splits.Any(s => s.Amount <= 0))
        {
            throw new ValidationException("Split amount must be greater than 0");
        }

        if (splits.Sum(s => s.Amount) != totalAmount)
        {
            throw new ValidationException("Total amount does not match with splits amount");
        }

        string orderNumber = DateTime.Now.ToString("ddyyMM") +
                             ShortId.Generate(new GenerationOptions(length: 8, useSpecialCharacters: false,
                                 useNumbers: false))[..4];

        Order order = new()
        {
            Id = Guid.NewGuid().ToString(),
            DeviceId = deviceId,
            TableId = tableId,
            SessionId = table.SessionId,
            OrderMenuItems = orderMenuItems,
            TotalAmount = totalAmount,
            OrderNumber = orderNumber,
        };
        Order? createdOrder = orderRepository.Create(order);

        splits.ForEach(s => s.OrderId = order.Id);
        bool hasCreatedSplits = splitRepository.CreateSplits(splits);
        if (!hasCreatedSplits || createdOrder == null)
        {
            throw new DatabaseCreationException("Order could not be created");
        }

        return createdOrder;
    }

    public List<Order>? GetByTableId(string tableId)
    {
        Table? table = tableRepository.GetById(tableId);
        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        if (!orderRepository.ExistsBySessionId(table.SessionId))
        {
            throw new NotFoundException("SessionId does not exist");
        }

        return orderRepository.GetByTableSessionId(table.SessionId);
    }

    public Order? GetBy(string id, string deviceId, string tableId)
    {
        if (!orderRepository.ExistsByDeviceId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        return orderRepository.GetBy(id, deviceId, tableId);
    }

    public Order? GetBy(string id)
    {
        return orderRepository.GetBy(id);
    }

    public bool Update(Order order)
    {
        return orderRepository.Update(order);
    }

    public void ProcessPaidOrder(Order order)
    {
        if (!cartItemRepository.ClearByDeviceId(order.DeviceId))
        {
            throw new DatabaseUpdateException("CartItems could not be cleared");
        }
    }

    public IEnumerable<Order> GetPaidOrders()
    {
        return orderRepository.GetPaidOrders();
    }

    public IEnumerable<Order> GetPaidFoodOrders()
    {
        IEnumerable<Order> orders = orderRepository.GetPaidOrders();

        IEnumerable<Order> foodOnlyOrders = orders.Select(order =>
            {
                order.OrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name != "Drinks"))
                    .ToList();

                return order;
            })
            .Where(order => order.OrderMenuItems.Count != 0);

        return foodOnlyOrders;
    }

    public IEnumerable<Order> GetPaidDrinksOrders()
    {
        IEnumerable<Order> orders = orderRepository.GetPaidOrders();

        IEnumerable<Order> drinkOnlyOrders = orders.Select(order =>
            {
                order.OrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name == "Drinks"))
                    .ToList();

                return order;
            })
            .Where(order => order.OrderMenuItems.Count != 0);

        return drinkOnlyOrders;
    }

    public int GetTotalAmount(string deviceId, string tableId)
    {
        if (!cartItemRepository.ExistsByTableSessionId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByTableSessionId(deviceId);
        if (cartItems.Count == 0)
        {
            throw new NotFoundException("CartItems do not exist");
        }

        return cartItems.Sum(item => item.MenuItem.Price * item.Quantity);
    }
}