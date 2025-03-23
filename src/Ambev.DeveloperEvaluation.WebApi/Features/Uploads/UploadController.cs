using Ambev.DeveloperEvaluation.Application.Uploads;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Uploads;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UploadController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponseWithData<UploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, CancellationToken cancellationToken = default)
    {
        UploadRequest request = new UploadRequest
        {
            File = file
        };

        var validator = new UploadRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UploadImageCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<UploadResponse>
        {
            Success = true,
            Message = "Upload created successfully",
            Data = _mapper.Map<UploadResponse>(response)
        });       
    }
}


