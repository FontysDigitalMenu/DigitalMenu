namespace DigitalMenu_10_Api.ViewModels;

public class CategoryViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public List<MenuItemViewModel> MenuItemViewModels { get; set; }
}