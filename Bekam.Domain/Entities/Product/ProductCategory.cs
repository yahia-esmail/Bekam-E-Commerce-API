using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Product;

public class ProductCategory : BaseAuditableEntity<int>
{
    public required string Name { get; set; }
    public string? PictureUrl { get; set; }
    public int? ParentCategoryId { get; set; }
    public ProductCategory? ParentCategory { get; set; }

    public ICollection<ProductCategory> SubCategories { get; set; }
        = new List<ProductCategory>();

    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}