using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageCommand : IRequest<UploadImageResult>
{
    public IFormFile File { get; set; }
}
