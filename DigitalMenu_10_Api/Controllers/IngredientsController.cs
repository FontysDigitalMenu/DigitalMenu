using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu_10_Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/ingredients")]
[ApiController]
public class IngredientsController(IIngredientService ingredientService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> GetIngredients()
    {
        List<Ingredient> ingredients = await ingredientService.GetIngredients();

        List<IngredientViewModel> ingredientViewModels = ingredients.Select(ingredient => new IngredientViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
        }).ToList();

        return Ok(ingredientViewModels);
    }
}