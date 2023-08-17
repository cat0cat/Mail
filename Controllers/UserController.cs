using Microsoft.AspNetCore.Mvc;
using YOApi.Data.Interfaces;
using YOApi.Models;
using YOApi.Sevice;

namespace YOApi.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
        

    public UserController(IUserRepository userRepository, IUserService userService, ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _userService = userService;
        this._logger = logger;
    }

    [HttpPost("create")]
    public async Task<ActionResult> RegistrationUser([FromBody]UserRequest user, CancellationToken token)
    {
        if (user.Address.Contains(' '))
            return BadRequest("Username cannot contain spaces");
        var check = await _userRepository.CreateUser(user,token);
        if (!check)
            return Conflict("Error database");
        return Ok();  
    }

    [HttpPost("login")]
    public async Task<ActionResult<int>> AuthenticateUser(UserRequest currentUser, CancellationToken token)
    {
        User? user = await _userRepository.GetUserByAddress(currentUser.Address, token);
        if (user is null || _userRepository.HashPassword(currentUser) != user.Password)
            return Forbid("Username or password is incorrect");
        var userToken = _userService.GenToken(user);
        _logger.LogInformation(userToken);
        return Ok(userToken);
    }
}