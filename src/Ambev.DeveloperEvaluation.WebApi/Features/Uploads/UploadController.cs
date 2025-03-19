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

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream); // Copia para um stream em memória

            if (memoryStream.Length == 0)
                return BadRequest("O arquivo está vazio após a cópia.");

            memoryStream.Position = 0; // Garante que a leitura começará do início

            string fileUrl = await _uploadImageHandler.Handle(memoryStream, file.FileName, file.ContentType);

            return Ok(new { imageUrl = fileUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao processar o upload da imagem", details = ex.Message });
        }
    }
}


