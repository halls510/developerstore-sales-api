using Ambev.DeveloperEvaluation.Domain.Services;
using MediatR;
using System.IO;
using System.Net.Mime;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageHandler : IRequestHandler<UploadImageCommand,string>
{
    private readonly IFileStorageService _fileStorageService;

    public UploadImageHandler(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    //public async Task<string> Handle(Stream fileStream, string fileName, string contentType)
    //{
    //    return await _fileStorageService.UploadFileAsync(fileStream, fileName, contentType);
    //}

    public async Task<string> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
       // var file = command.File;
       // using var memoryStream = new MemoryStream();
       // Console.WriteLine("Iniciando cópia do arquivo...");
       // await file.CopyToAsync(memoryStream);
       // Console.WriteLine($"Arquivo copiado com sucesso! Tamanho: {memoryStream.Length} bytes");


       //// if (memoryStream.Length == 0)
            

       // memoryStream.Position = 0; // Garante que a leitura começará do início

       // return await _fileStorageService.UploadFileAsync(memoryStream, file.FileName, file.ContentType);
        return await _fileStorageService.UploadFileAsync(command.File);
    }
}