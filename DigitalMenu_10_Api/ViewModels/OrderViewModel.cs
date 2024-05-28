using DigitalMenu_20_BLL.Enums;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;

namespace DigitalMenu_10_Api.ViewModels;

public class OrderViewModel
{
    public string Id { get; set; }

    public int TotalAmount { get; set; }

    public string DrinkStatus { get; set; }

    public string FoodStatus { get; set; }

    public bool IsPaymentSuccess { get; set; }

    public DateTime OrderDate { get; set; } = DateTimeService.GetNow();

    public List<MenuItemViewModel> MenuItems { get; set; } = [];

    public string OrderNumber { get; set; }

    public List<SplitViewModel>? Splits { get; set; }

    public static OrderViewModel FromOrder(Order order, ICartItemService cartItemService)
    {
        return new OrderViewModel
        {
            Id = order.Id,
            IsPaymentSuccess = order.Splits.All(s => s.PaymentStatus == PaymentStatus.Paid),
            FoodStatus = order.FoodStatus.ToString(),
            DrinkStatus = order.DrinkStatus.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
            Splits = order.Splits.Select(s => new SplitViewModel
            {
                Id = s.Id,
                Amount = s.Amount,
                Name = s.Name,
                PaymentStatus = s.PaymentStatus.ToString(),
            }).ToList(),
        };
    }

    public static OrderViewModel FromOrderWithCatagory(Order order, ICartItemService cartItemService)
    {
        return new OrderViewModel
        {
            Id = order.Id,
            IsPaymentSuccess = order.Splits.All(s => s.PaymentStatus == PaymentStatus.Paid),
            FoodStatus = order.FoodStatus.ToString(),
            DrinkStatus = order.DrinkStatus.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                Categories = omi.MenuItem.CategoryMenuItems
                    .Where(cmi => cmi.Category != null)
                    .Select(cmi => cmi.Category!.Name)
                    .ToList(),
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
            Splits = order.Splits.Select(s => new SplitViewModel
            {
                Id = s.Id,
                Amount = s.Amount,
                Name = s.Name,
                PaymentStatus = s.PaymentStatus.ToString(),
            }).ToList(),
        };
    }
}