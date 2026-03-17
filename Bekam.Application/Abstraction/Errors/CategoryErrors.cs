using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public class CategoryErrors
{
    public static readonly Error CategoryNotFound =
        new("Category.CategoryNotFound", "Category not found.", ErrorType.NotFound);

    public static readonly Error ParentNotFound =
        new("Category.ParentNotFound", "Parent Category not found.", ErrorType.NotFound);

    public static readonly Error CategoryHasSubCategories =
        new("Category.CategoryHasSubCategories", "Not allowed to remove Category Has Sub Categories.", ErrorType.Validation);

    public static readonly Error CategoryHasProducts =
        new("Category.CategoryHasProducts", "Not allowed to remove Category Has Products.", ErrorType.Validation);

    public static readonly Error DuplicateCategoryName =
        new("Category.DuplicateCategoryName", "Category with this name is already existed.", ErrorType.Conflict);
}
