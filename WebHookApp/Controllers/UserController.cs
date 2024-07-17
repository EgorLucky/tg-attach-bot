using DomainLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebHookApp.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserController : Base.BaseController
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }


    [HttpGet("current")]
    public async Task<IActionResult> Get()
    {
        var user = await _userService.GetUser(UserId);
        return Response(user);
    }
}