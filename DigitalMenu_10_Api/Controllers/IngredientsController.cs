using DigitalMenu_10_Api.RequestModels;
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
            Stock = ingredient.Stock,
        }).ToList();

        return Ok(ingredientViewModels);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> GetIngredientById([FromRoute] int id)
    {
        Ingredient? ingredient = await ingredientService.GetIngredientById(id);
        if (ingredient == null)
        {
            return NotFound("Ingredient not found.");
        }

        IngredientViewModel ingredientViewModel =
            new() { Id = ingredient.Id, Name = ingredient.Name, Stock = ingredient.Stock };

        return Ok(ingredientViewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> CreateIngredient([FromBody] IngredientCreateRequest ingredientCreateRequest)
    {
        try
        {
            Ingredient ingredient = new()
            {
                Name = ingredientCreateRequest.Name,
                Stock = ingredientCreateRequest.Stock,
            };

            Ingredient? createdIngredient = await ingredientService.CreateIngredient(ingredient);
            if (createdIngredient == null)
            {
                return BadRequest(new { Message = "Ingredient could not be created" });
            }

            return Created("",
                new Ingredient
                    { Id = createdIngredient.Id, Name = createdIngredient.Name, Stock = createdIngredient.Stock });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> UpdateIngredient(int id, [FromBody] IngredientUpdateRequest ingredientUpdateRequest)
    {
        try
        {
            Ingredient? ingredient = await ingredientService.GetIngredientById(id);
            if (ingredient == null)
            {
                return NotFound("Ingredient not found.");
            }

            ingredient.Name = ingredientUpdateRequest.Name;
            ingredient.Stock = ingredientUpdateRequest.Stock;

            bool ingredientUpdated = await ingredientService.UpdateIngredient(ingredient);
            if (!ingredientUpdated)
            {
                return BadRequest(new { Message = "Ingredient could not be updated" });
            }

            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
    }
}