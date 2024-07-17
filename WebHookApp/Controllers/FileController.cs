using DomainLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}