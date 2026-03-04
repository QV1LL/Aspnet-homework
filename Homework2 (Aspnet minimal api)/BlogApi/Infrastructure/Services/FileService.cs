using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BlogApi.Infrastructure.Services;

public class FileService(IWebHostEnvironment environment) : IFileService
{
    private const string ImagesFolder = "images";

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        var uploadsPath = Path.Combine(environment.WebRootPath, ImagesFolder);

        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/{uploadsPath}/{fileName}";
    }

    public bool DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return false;

        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(environment.WebRootPath, relativePath);

        if (!File.Exists(fullPath)) return false;
        
        File.Delete(fullPath);
        return true;

    }
}