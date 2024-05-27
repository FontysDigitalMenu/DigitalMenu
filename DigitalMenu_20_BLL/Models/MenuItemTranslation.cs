namespace DigitalMenu_20_BLL.Models;

public class MenuItemTranslation
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public string LanguageCode { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}