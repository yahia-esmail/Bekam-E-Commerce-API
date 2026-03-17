using Bekam.Application.Abstraction.Contracts.Services.Helpers;

namespace Bekam.API.Services;

public class UrlBuilder : IUrlBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public string BaseUrl { get; init; }

    public UrlBuilder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        var request = _httpContextAccessor.HttpContext?.Request;

        BaseUrl = $"{request?.Scheme}://{request?.Host}";
    }
    public string BuildPictureUrl(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        return $"{BaseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }
}
