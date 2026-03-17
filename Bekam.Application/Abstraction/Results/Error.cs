namespace Bekam.Application.Abstraction.Results;

public record Error(
    string Code,
    string Description,
    ErrorType? Type,
    IReadOnlyDictionary<string, string[]>? Metadata = null
)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}

public enum ErrorType
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Failure
}
