namespace BlogApi.Infrastructure.Services;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file);
    bool DeleteFile(string fileUrl);
}