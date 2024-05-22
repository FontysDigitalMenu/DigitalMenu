namespace DigitalMenu_20_BLL.Models;

public class CategoryTranslation
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }

    public string LanguageCode { get; set; }

    public string Name { get; set; }
}