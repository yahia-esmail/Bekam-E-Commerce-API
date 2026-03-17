using Bekam.Domain.Entities.Product;

namespace Bekam.Domain.Specifications.Categories;
public class ParentCategoriesSpecifications
    : BaseSpecifications<ProductCategory, int>
{
    public ParentCategoriesSpecifications():
        base(c => c.ParentCategoryId == null)
    {
        
    }
}