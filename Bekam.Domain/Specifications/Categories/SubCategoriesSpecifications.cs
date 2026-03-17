using Bekam.Domain.Entities.Product;

namespace Bekam.Domain.Specifications.Categories;
public class SubCategoriesSpecifications
    : BaseSpecifications<ProductCategory, int>
{
    public SubCategoriesSpecifications(int parentId) :
        base(c => c.ParentCategoryId == parentId)
    {

    }
}