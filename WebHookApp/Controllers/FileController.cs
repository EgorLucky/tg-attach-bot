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
    [HttpGet("download/{filePath}")]
    public async Task GetFile(
        [FromRoute] string filePath,
        [FromQuery] bool isImage,
        [FromServices] BotConfiguration botConfiguration, 
        [FromServices] IHttpClientFactory httpClientFactory)
    {
        var httpClient = httpClientFactory.CreateClient();

        var httpRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new($"https://api.telegram.org/file/bot{botConfiguration.BotToken}/{filePath}"),
        };
        
        var rangeHeader = Request.Headers.Range;
        if (rangeHeader.Any())
        {
            httpRequest.Headers.Range = RangeHeaderValue.Parse(rangeHeader.First());
        }

        var response = await httpClient.SendAsync(httpRequest);

        if (response.Content.Headers.ContentRange is not null)
            ControllerContext.HttpContext.Response.Headers.ContentRange =
                response.Content.Headers.ContentRange.ToString();
        if (response.Content.Headers.ContentType is not null && !isImage)
            ControllerContext.HttpContext.Response.Headers.ContentType =
                response.Content.Headers.ContentType.ToString();
        
        await response.Content.CopyToAsync(HttpContext.Response.Body);
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
}