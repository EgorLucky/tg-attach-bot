using System.Net.Http.Headers;
using DomainLogic.DTOs.Input;
using DomainLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;

namespace WebHookApp.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class FileController : Base.BaseController
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var file = await _fileService.GetFileById(id, UserId);
        return Response(file);
    }
    
    [AllowAnonymous]
    [HttpGet("download/{fileId}")]
    public async Task GetFile(
        [FromRoute] string fileId,
        [FromQuery] bool isImage,
        [FromServices] TelegramFileDownloadService telegramFileDownloadService)
    {
        await telegramFileDownloadService.DownloadFile(
            fileId,
            Request.Headers.Range.FirstOrDefault(),
            fileMetaData =>
            {
                var responseHeaders = ControllerContext.HttpContext.Response.Headers;
                if (fileMetaData.ContentRange is not null)
                    responseHeaders.ContentRange = fileMetaData.ContentRange;
                if (fileMetaData.ContentType is not null && !isImage)
                    responseHeaders.ContentType = fileMetaData.ContentType;
                HttpContext.Response.StatusCode = fileMetaData.StatusCode;
            },
            HttpContext.Response.Body);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UpdateFileDTO dto)
    {
        var result = await _fileService.Update(dto, UserId);
        return Response(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _fileService.Delete(id, UserId);
        return Response(result);
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] FileListParameterDTO parameters)
    {
        var result = await _fileService.GetList(parameters, UserId);
        return Response(result);
    }
}