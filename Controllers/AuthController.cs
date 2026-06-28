
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models.Dtos.Auth;
using TaskTracker.Services;
namespace TaskTracker.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController:ControllerBase
  {
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
      _authService=authService;
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
