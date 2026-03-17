namespace Bekam.Application.DTOs.categories;
public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? PictureUrl { get; set; }
    public int? ParentCategoryId { get; set; }
}
