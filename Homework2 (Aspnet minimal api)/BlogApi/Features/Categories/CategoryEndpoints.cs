using BlogApi.Domain.Entities;

namespace BlogApi.Features.Categories;

public static class CategoryEndpoints
{
     public static void MapCategories(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories");

        group.MapGet("/", async (CategoryService categoryService) =>
        {
            var categories = await categoryService.GetCategoriesAsync();
            
            return Results.Ok(categories);
        });

        group.MapGet("/{id:Guid}", async (CategoryService categoryService, Guid id) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);

            return category is null 
                ? Results.NotFound("Category not found") 
                : Results.Ok(category);
        });

        group.MapPost("/", async (CategoryService categoryService, CreateCategory dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Results.UnprocessableEntity("Category name is required");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

            await categoryService.AddCategoryAsync(category);
            
            return Results.Created($"/api/categories/{category.Id}", category);
        });

        group.MapPut("/", async (CategoryService categoryService, UpdateCategory dto) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return Results.UnprocessableEntity("Category name is required");
            }

            var existingCategory = await categoryService.GetCategoryByIdAsync(dto.Id);
            
            if (existingCategory is null)
            {
                return Results.NotFound("Category not found");
            }

            existingCategory.Name = dto.Name;
            await categoryService.UpdateCategoryAsync(existingCategory);

            return Results.NoContent();
        });

        group.MapDelete("/{id:Guid}", async (CategoryService categoryService, Guid id) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            
            if (category is null)
            {
                return Results.NotFound("Category not found");
            }

            await categoryService.DeleteCategoryAsync(id);

            return Results.NoContent();
        });
    }
}