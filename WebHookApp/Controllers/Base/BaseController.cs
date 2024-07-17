using System.Security.Claims;
using DomainLogic.DTOs.Output;
using Microsoft.AspNetCore.Mvc;

namespace WebHookApp.Controllers.Base;

public abstract class BaseController: ControllerBase
{
    protected long UserId => long.Parse(User.Claims.First(x => x.Type == "userId").Value);

    protected IActionResult Response(BaseResultDTO result)
    {
        if (result.Success) return Ok();
        return BadRequest(result.ErrorMessage);
    }

    protected IActionResult Response<T>(BaseResultObjectDataDTO<T> result) where T: class
    {
        if (result.Success) return Ok(result.Result);
        return BadRequest(result.ErrorMessage);
    }

    protected IActionResult Response<T>(BaseResultStructDataDTO<T> result) where T : struct
    {
        if (result.Success) return Ok(result.Result);
        return BadRequest(result.ErrorMessage);
    }
}