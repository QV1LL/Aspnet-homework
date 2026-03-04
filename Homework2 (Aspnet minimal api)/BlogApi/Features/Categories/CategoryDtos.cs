namespace BlogApi.Features.Categories;

public record CreateCategory(string Name);
public record UpdateCategory(Guid Id, string Name);