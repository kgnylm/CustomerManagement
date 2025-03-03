using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerManagement.Data;
using CustomerManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Services;

public class CategoryService : ICategoryService
{
    private readonly CustomerManagementContext _context;

    public CategoryService(CustomerManagementContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Include(c => c.Customers)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Customers)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
        if (existingCategory == null)
        {
            throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");
        }

        existingCategory.CategoryName = category.CategoryName;
        existingCategory.CategoryDescription = category.CategoryDescription;

        await _context.SaveChangesAsync();
        return existingCategory;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
} 