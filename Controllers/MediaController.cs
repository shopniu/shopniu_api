using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopniu_api.Aplication.Media;
using Shopniu_api.Aplication.Media.UseCases.ConfirmMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.CreateMediaUpload;
using Shopniu_api.Aplication.Media.UseCases.LinkMedia;

namespace Shopniu_api.Routes;

[ApiController]
[Route("api/v1/media")]
[Authorize(Policy = "product.create")]
public class MediaController : ControllerBase
{
    private readonly MediaHandler _mediaHandler;

    public MediaController(MediaHandler mediaHandler)
    {
        _mediaHandler = mediaHandler;
    }

    // El front pide una SAS de escritura efímera, sube el archivo directo a
    // Blob Storage y luego confirma con el POST /media.
    [HttpPost("upload-url")]
    public async Task<IActionResult> CreateUploadUrl([FromBody] CreateMediaUploadRequest dto)
    {
        return Ok(await _mediaHandler.CreateUploadUrlAsync(dto, HttpContext.RequestAborted));
    }

    // Confirma el upload: valida la imagen, genera variantes (web/thumb) y
    // persiste la MediaAsset. Opcionalmente la vincula a un producto.
    [HttpPost]
    public async Task<IActionResult> ConfirmMedia([FromBody] ConfirmMediaUploadRequest dto)
    {
        return Ok(await _mediaHandler.ConfirmMediaAsync(dto, HttpContext.RequestAborted));
    }

    // Marca la imagen como principal de su producto (sincroniza ImageUrl).
    [HttpPost("{id:int}/main")]
    public async Task<IActionResult> SetMain(int id)
    {
        return Ok(await _mediaHandler.SetMainAsync(id, HttpContext.RequestAborted));
    }

    // Vincula media huérfana a un producto (flujo "crear producto").
    [HttpPost("link")]
    public async Task<IActionResult> LinkMedia([FromBody] LinkMediaRequest dto)
    {
        return Ok(await _mediaHandler.LinkMediaAsync(dto, HttpContext.RequestAborted));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMedia(int id)
    {
        return Ok(await _mediaHandler.DeleteMediaAsync(id, HttpContext.RequestAborted));
    }
}
