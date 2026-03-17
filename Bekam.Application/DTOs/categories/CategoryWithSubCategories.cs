namespace Bekam.Application.DTOs.categories;
public class CategoryWithSubCategories
{
    public required int Id { get; set; }
    public required string Name { get; set; } 
    public string? PictureUrl { get; set; }
    public IEnumerable<CategoryDto> SubCategories { get; set; }
}
