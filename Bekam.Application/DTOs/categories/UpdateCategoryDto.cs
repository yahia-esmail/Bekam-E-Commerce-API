using Microsoft.AspNetCore.Http;

namespace Bekam.Application.DTOs.categories;
public class UpdateCategoryDto
{
    public required string Name { get; set; }
    public IFormFile? Picture { get; set; }
    public int? ParentCategoryId { get; set; }
}
