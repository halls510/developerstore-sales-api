using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageCommand : IRequest<string>
{
    public IFormFile File { get; set; }
}
