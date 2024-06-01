using DigitalMenu_10_Api.Hub;
using DigitalMenu_10_Api.RequestModels;
using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace DigitalMenu_10_Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/ingredients")]
[ApiController]
public class IngredientsController(
    IIngredientService ingredientService,
    IHubContext<IngredientHub, IIngredientHubClient> hubContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> GetIngredients()
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        List<Ingredient> ingredients = await ingredientService.GetIngredients();

        List<IngredientViewModel> ingredientViewModels = ingredients.Select(ingredient => new IngredientViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Name ?? ingredient.Name,
            Stock = ingredient.Stock,
        }).ToList();

        return Ok(ingredientViewModels);
    }

    [HttpGet("paginated")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> GetIngredientsPaginated(int currentPage, int amount)
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        List<Ingredient> ingredients = await ingredientService.GetIngredientsPerPage(currentPage, amount);

        List<IngredientViewModel> ingredientViewModels = ingredients.Select(ingredient => new IngredientViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Name ?? ingredient.Name,
            Stock = ingredient.Stock,
        }).ToList();

        return Ok(ingredientViewModels);
    }

    [HttpGet("count")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public ActionResult GetCount()
    {
        int ingredientCount = ingredientService.GetIngredientCount();
        return Ok(ingredientCount);
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
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        try
        {
            Ingredient ingredient = new()
            {
                Name = ingredientCreateRequest.Name,
                Stock = ingredientCreateRequest.Stock,
            };

            Ingredient? createdIngredient =
                await ingredientService.CreateIngredient(ingredient, localeValue);
            if (createdIngredient == null)
            {
                return BadRequest(new { Message = "Ingredient could not be created" });
            }

            await SendUpdatedIngredientsStock();

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
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        try
        {
            Ingredient? ingredient = await ingredientService.GetIngredientById(id);
            if (ingredient == null)
            {
                return NotFound("Ingredient not found.");
            }

            ingredient.Name = ingredientUpdateRequest.Name;
            ingredient.Stock = ingredientUpdateRequest.Stock;

            bool ingredientUpdated =
                await ingredientService.UpdateIngredient(ingredient, localeValue);
            if (!ingredientUpdated)
            {
                return BadRequest(new { Message = "Ingredient could not be updated" });
            }

            await SendUpdatedIngredientsStock();

            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> DeleteIngredient(int id)
    {
        try
        {
            bool isIngredientDeleted = await ingredientService.DeleteIngredient(id);

            if (!isIngredientDeleted)
            {
                return BadRequest(new { Message = "Could not delete ingredient." });
            }

            await SendUpdatedIngredientsStock();

            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { e.Message });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    private async Task SendUpdatedIngredientsStock()
    {
        List<Ingredient> ingredients = await ingredientService.GetIngredients();

        List<IngredientViewModel> ingredientViewModels = ingredients.Select(ingredient => new IngredientViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Stock = ingredient.Stock,
        }).ToList();

        await hubContext.Clients.All.ReceiveIngredient(ingredientViewModels);
    }
}