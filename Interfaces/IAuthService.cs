using TaskTracker.Models.Dtos.Auth;

namespace TaskTracker.Interfaces
{
  public interface IAuthService
  {
    Task<(bool success, string message)> RegisterAsync(RegisterRequest request);
    Task<(bool success, string message, AuthResponse? data)> LoginAsync(LoginRequest request);
  }
}