using Microsoft.AspNetCore.Http;
using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Contracts.Services.Helpers;
public interface IImageService
{
    Task<Result<string>> UploadImageAsync(IFormFile file, string folderName);
    void DeleteImage(string imagePath);
}