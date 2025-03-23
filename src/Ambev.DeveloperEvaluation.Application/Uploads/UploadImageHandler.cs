using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageHandler : IRequestHandler<UploadImageCommand, UploadImageResult>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<UploadImageHandler> _logger;

    public UploadImageHandler(IFileStorageService fileStorageService,
         IMapper mapper,
        ILogger<UploadImageHandler> logger)
    {
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }   

    public async Task<UploadImageResult> Handle(UploadImageCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing upload: {Filename}", command.File.FileName);

        var validator = new UploadImageCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for upload: {Filename}", command.File.FileName);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Upload created successfully: {Filename}", command.File.FileName);
       
        var result = await _fileStorageService.UploadFileAsync(command.File);

        return _mapper.Map<UploadImageResult>(result);
    }
}