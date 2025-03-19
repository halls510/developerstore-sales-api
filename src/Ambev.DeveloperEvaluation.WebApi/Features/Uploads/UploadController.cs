using Ambev.DeveloperEvaluation.Application.Uploads;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Uploads;

[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly UploadImageHandler _uploadImageHandler;

    public UploadController(UploadImageHandler uploadImageHandler)
    {
        _uploadImageHandler = uploadImageHandler;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Nenhuma imagem foi enviada.");

        using var stream = file.OpenReadStream();
        string fileUrl = await _uploadImageHandler.Handle(stream, file.FileName, file.ContentType);

        return Ok(new { imageUrl = fileUrl });
    }
}
