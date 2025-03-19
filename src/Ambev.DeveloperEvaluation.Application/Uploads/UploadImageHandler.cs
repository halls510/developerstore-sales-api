using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageHandler
{
    private readonly IFileStorageService _fileStorageService;

    public UploadImageHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<string> Handle(Stream fileStream, string fileName, string contentType)
    {
        return await _fileStorageService.UploadFileAsync(fileStream, fileName, contentType);
    }
}