using Bekam.Domain.Entities.Product;

namespace Bekam.Domain.Specifications.Products;
public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product, int>
{
    public ProductWithBrandAndCategorySpecifications(string? sort, int? brandId, int? categoryId, int pageSize, int pageNumber, string? search)
        : base(

              P =>
                    (string.IsNullOrEmpty(search) || P.NormalizedName.Contains(search))
                        &&
                    (!brandId.HasValue || P.BrandId == brandId.Value)
                        &&
                    (!categoryId.HasValue || P.CategoryId == categoryId.Value)

              )
    {
        AddIncludes();

        switch (sort)
        {
            case "nameDesc":
                ApplyOrderByDescending(P => P.Name);
                break;

            case "priceAsc":
                ApplyOrderBy(P => P.Price);
                break;

            case "priceDesc":
                ApplyOrderByDescending(P => P.Price);
                break;

            case "newest":
                ApplyOrderByDescending(P => P.CreatedOn);
                break;

            default:
                ApplyOrderBy(P => P.Name);
                break;
        }


        ApplyPaging(pageSize * (pageNumber - 1), pageSize);

    }

    public ProductWithBrandAndCategorySpecifications(int id)
        : base(id)
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        AddInclude(P => P.Category!);
        AddInclude(P => P.Brand!);
    }
}

