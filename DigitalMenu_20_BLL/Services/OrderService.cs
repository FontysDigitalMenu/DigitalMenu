using System.ComponentModel.DataAnnotations;
using DigitalMenu_20_BLL.Enums;
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
    ISplitRepository splitRepository,
    IMenuItemRepository menuItemRepository,
    IIngredientRepository ingredientRepository) : IOrderService
{
    public async Task<Order> Create(string tableSessionId, List<Split> splits)
    {
        Table? table = tableRepository.GetBySessionId(tableSessionId);
        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByTableSessionId(tableSessionId);
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


        foreach (OrderMenuItem orderMenuItem in orderMenuItems)
        {
            MenuItem? menuItem = menuItemRepository.GetMenuItemBy(orderMenuItem.MenuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException("Menu item does not exist");
            }
            
            foreach (Ingredient ingredient in menuItem.Ingredients)
            {
                Ingredient? ingredientWithStock = await ingredientRepository.GetIngredientById(ingredient.Id);

                if (ingredientWithStock != null)
                {
                    ingredient.Stock = ingredientWithStock.Stock;
                }

                if (ingredient.Stock < (ingredient.Pieces * orderMenuItem.Quantity) || ingredient.Stock - (ingredient.Pieces * orderMenuItem.Quantity) < 0)
                {
                    throw new Exception("Insufficient stock for ingredient: " + ingredient.Name);
                }
            }

            foreach (Ingredient ingredient in menuItem.Ingredients)
            {
                Ingredient? ingredientWithStock = await ingredientRepository.GetIngredientById(ingredient.Id);

                if (ingredientWithStock != null)
                {
                    ingredient.Stock = ingredientWithStock.Stock;
                }

                ingredient.Stock -= ingredient.Pieces * orderMenuItem.Quantity;

                Ingredient updatedIngredient = new()
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Stock = ingredient.Stock,
                };

                await ingredientRepository.UpdateIngredient(updatedIngredient);
            }
        }

        int totalAmount = GetTotalAmount(tableSessionId);
        if (splits.Any(s => s.Amount <= 0))
        {
            throw new ValidationException("Split amount must be greater than 0");
        }

        if (splits.Sum(s => s.Amount) != totalAmount)
        {
            throw new ValidationException("Total amount does not match with splits amount");
        }

        string orderNumber = DateTimeService.GetNow().ToString("ddyyMM") +
                             ShortId.Generate(new GenerationOptions(length: 8, useSpecialCharacters: false,
                                 useNumbers: false))[..4];

        Order order = new()
        {
            Id = Guid.NewGuid().ToString(),
            TableId = table.Id,
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

    public List<Order>? GetByTableSessionId(string tableSessionId)
    {
        Table? table = tableRepository.GetBySessionId(tableSessionId);
        if (table == null)
        {
            throw new NotFoundException("Table Session Id does not exist");
        }

        if (!orderRepository.ExistsBySessionId(table.SessionId))
        {
            throw new NotFoundException("SessionId does not exist");
        }

        return orderRepository.GetByTableSessionId(table.SessionId);
    }

    public Order? GetBy(string id, string tableSessionId)
    {
        if (tableRepository.GetBySessionId(tableSessionId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        return orderRepository.GetBy(id, tableSessionId);
    }

    public Order? GetBy(string id)
    {
        return orderRepository.GetBy(id);
    }

    public bool Update(Order order)
    {
        return orderRepository.Update(order);
    }

    public List<Order> GetUnpaidOrdersByTableSessionId(string tableSessionId)
    {
        return orderRepository.GetUnPaidOrders(tableSessionId);
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
                List<OrderMenuItem> foodOrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name != "Drinks"))
                    .ToList();

                List<OrderMenuItem> drinkOrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name == "Drinks"))
                    .ToList();

                if (drinkOrderMenuItems.Count <= 0)
                {
                    order.DrinkStatus = OrderStatus.None;
                }

                order.OrderMenuItems = foodOrderMenuItems;

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
                List<OrderMenuItem> foodOrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name != "Drinks"))
                    .ToList();

                List<OrderMenuItem> drinkOrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name == "Drinks"))
                    .ToList();

                if (foodOrderMenuItems.Count <= 0)
                {
                    order.FoodStatus = OrderStatus.None;
                }

                order.OrderMenuItems = drinkOrderMenuItems;

                return order;
            })
            .Where(order => order.OrderMenuItems.Count != 0);

        return drinkOnlyOrders;
    }

    public IEnumerable<Order> GetCompletedOrders()
    {
        IEnumerable<Order> completedOrders = orderRepository.GetPaidOrders()
            .Where(o => o.FoodStatus == OrderStatus.Completed || o.DrinkStatus == OrderStatus.Completed);

        return completedOrders;
    }

    public IEnumerable<Order> GetCompletedFoodOrders()
    {
        IEnumerable<Order> orders = orderRepository.GetPaidOrders();

        IEnumerable<Order> completedFoodOnlyOrders = orders.Select(order =>
            {
                order.OrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name != "Drinks")
                                  && omi.Order.FoodStatus == OrderStatus.Completed)
                    .ToList();

                return order;
            })
            .Where(order => order.OrderMenuItems.Count != 0);

        return completedFoodOnlyOrders;
    }

    public IEnumerable<Order> GetCompletedDrinksOrders()
    {
        IEnumerable<Order> orders = orderRepository.GetPaidOrders();

        IEnumerable<Order> completedDrinksOnlyOrders = orders.Select(order =>
            {
                order.OrderMenuItems = order.OrderMenuItems
                    .Where(omi => omi.MenuItem.CategoryMenuItems.Any(c => c.Category.Name == "Drinks")
                                  && omi.Order.DrinkStatus == OrderStatus.Completed)
                    .ToList();

                return order;
            })
            .Where(order => order.OrderMenuItems.Count != 0);

        return completedDrinksOnlyOrders;
    }

    public int GetTotalAmount(string tableSessionId)
    {
        if (!cartItemRepository.ExistsByTableSessionId(tableSessionId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetBySessionId(tableSessionId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByTableSessionId(tableSessionId);
        if (cartItems.Count == 0)
        {
            throw new NotFoundException("CartItems do not exist");
        }

        return cartItems.Sum(item => item.MenuItem.Price * item.Quantity);
    }
}