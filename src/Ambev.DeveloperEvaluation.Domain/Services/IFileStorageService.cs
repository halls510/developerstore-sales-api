namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
}
