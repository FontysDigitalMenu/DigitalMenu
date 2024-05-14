using DigitalMenu_10_Api.ViewModels;

namespace DigitalMenu_10_Api.Hub;

public interface IIngredientHubClient
{
    Task ReceiveIngredient(List<IngredientViewModel> ingredientViewModels);
}