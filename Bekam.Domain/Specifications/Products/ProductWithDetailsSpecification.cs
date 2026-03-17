using Bekam.Domain.Entities.Product;

namespace Bekam.Domain.Specifications.Products;
public class ProductWithDetailsSpecification
    : BaseSpecifications<Product, int>
{
    public ProductWithDetailsSpecification(int productId)
        : base(p => p.Id == productId)
    {
        AddInclude(p => p.Category!);
        AddInclude(p => p.Brand!);
    }
}
