namespace DigitalMenu_20_BLL.Models;

public class IngredientTranslation
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; }

    public string LanguageCode { get; set; }

    public string Name { get; set; }
}