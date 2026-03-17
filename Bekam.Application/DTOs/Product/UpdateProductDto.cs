using Microsoft.AspNetCore.Http;

namespace Bekam.Application.DTOs.Product;
public class UpdateProductDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public IFormFile? Picture { get; set; }
    public decimal Price { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }
}
