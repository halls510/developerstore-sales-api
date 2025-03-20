using Ambev.DeveloperEvaluation.Application.Uploads;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Uploads;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class UploadController : ControllerBase
{
   // private readonly UploadImageHandler _uploadImageHandler;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    //public UploadController(UploadImageHandler uploadImageHandler)
    //{
    //    _uploadImageHandler = uploadImageHandler;
    //}


    public UploadController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(PaginatedList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Nenhuma imagem foi enviada.");

        UploadRequest request = new UploadRequest
        {
            File = file
        };


        var command = _mapper.Map<UploadImageCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<string>
        {
            Success = true,
            Message = "Upload created successfully",
            Data = _mapper.Map<string>(response)
        });       
    }
}


