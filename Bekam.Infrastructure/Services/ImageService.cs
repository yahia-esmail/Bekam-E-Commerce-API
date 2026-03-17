using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;

namespace Bekam.Infrastructure.Services;
public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _env;

    public ImageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public void DeleteImage(string imagePath)
    {
        var fullPath = Path.Combine(_env.WebRootPath, imagePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, string folderName)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();

        var fileName = Guid.NewGuid() + extension;

        var folderPath = Path.Combine(_env.WebRootPath, "images", folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Result.Success($"images/{folderName}/{fileName}");
    }
}