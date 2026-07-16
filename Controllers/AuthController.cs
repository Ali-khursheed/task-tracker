
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models.Dtos.Auth;
using TaskTracker.Interfaces;

using TaskTracker.Repositories;
using TaskTracker.Services;
namespace TaskTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController:ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepo;


    public AuthController(AuthService authService, UserRepository userRepo)
    {
      _authService=authService;
      _userRepo=userRepo;
    }





    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _userRepo.GetAllAsync();
      return Ok(users.Select(u => new { u.Id, u.Username }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
      var (success, message)=await _authService.RegisterAsync(request);

      if(!success)
        return BadRequest(new { message });

      return Ok(new { message });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
      var (success, message, data)=await _authService.LoginAsync(request);

      if(!success)
        return Unauthorized(new { message });

      return Ok(data);
    }
  }
}
