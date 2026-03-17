using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Product;

public class ProductBrand : BaseAuditableEntity<int>
{
    public required string Name { get; set; }
    public required string NormalizedName { get; set; }
    public ICollection<Product> Products { get; set; }
        = new List<Product>();
}