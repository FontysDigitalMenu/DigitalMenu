namespace DigitalMenu_10_Api.ViewModels;

public class ReceiveOrderViewModel
{
    public string TableId { get; set; }

    public List<MenuItemViewModel> MenuItems { get; set; } = [];
}