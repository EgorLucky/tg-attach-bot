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
    public async Task<Object> GetFile([FromRoute] string fileId, [FromServices] ITelegramBotClient botClient)
    {
        try
        {
            var stream = new MemoryStream();
            var file = await botClient.GetInfoAndDownloadFileAsync(fileId, stream);
            stream.Position = 0;
            return stream;
        }
        catch (Exception ex) { return null; }
    }
}