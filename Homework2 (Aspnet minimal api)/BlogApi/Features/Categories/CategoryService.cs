using BlogApi.Domain.Entities;
using BlogApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Features.Categories;

public class CategoryService(BlogDbContext context)
{
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesByIdsAsync(IEnumerable<Guid> ids)
    {
        return await context.Categories
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid id)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddCategoryAsync(Category category)
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        context.Entry(category).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category != null)
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
    }
}