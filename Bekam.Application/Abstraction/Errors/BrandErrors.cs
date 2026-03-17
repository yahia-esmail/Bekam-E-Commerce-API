using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public class BrandErrors
{
    public static readonly Error NotFound =
       new("Brand.BrandNotFound", "Brand not found.", ErrorType.NotFound);

    public static readonly Error DuplicateName =
        new("Brand.DuplicateName", "Brand with this name is already existed.", ErrorType.Conflict);
}
