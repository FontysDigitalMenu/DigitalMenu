﻿using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<Category>> GetCategories()
    {
        return await categoryRepository.GetCategories();
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new NotFoundException("Category name is empty");
        }

        return await categoryRepository.GetCategoryByName(categoryName);
    }
}