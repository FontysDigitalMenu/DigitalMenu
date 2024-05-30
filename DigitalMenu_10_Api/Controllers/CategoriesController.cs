using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace DigitalMenu_10_Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/categories")]
[ApiController]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> GetCategories()
    {
        Request.Headers.TryGetValue("Accept-Language", out StringValues locale);
        string localeValue = locale.FirstOrDefault() ?? "en";
        if (localeValue.Length > 2) localeValue = "en";

        List<Category> categories = await categoryService.GetCategories();

        List<CategoryViewModel> menuItemViewModels = categories.Select(category => new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Translations?.FirstOrDefault(t => t.LanguageCode == localeValue)?.Name ?? category.Name,
        }).ToList();

        return Ok(menuItemViewModels);
    }
}