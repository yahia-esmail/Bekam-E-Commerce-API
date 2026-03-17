using Microsoft.AspNetCore.Identity;

namespace Bekam.Application.Abstraction.Results;
public static class IdentityErrorMapper
{
    public static Error ToError(IEnumerable<IdentityError> errors)
    {
        var metadata = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );

        return new Error(
            "Identity.Validation",
            "One or more validation errors occurred.",
            ErrorType.Validation,
            metadata
        );
    }
}