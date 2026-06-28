using TaskTracker.Helpers;
using TaskTracker.Models.Dtos.Auth;
using TaskTracker.Models.Entities;
using TaskTracker.Repositories;

namespace TaskTracker.Services
{
  public class AuthService
  {
    private readonly UserRepository _userRepo;
    private readonly JwtHelper _jwtHelper;

    public AuthService(UserRepository userRepo, JwtHelper jwtHelper)
    {
      _userRepo=userRepo;
      _jwtHelper=jwtHelper;
    }
  

    public async Task<(bool success, string message)> RegisterAsync(RegisterRequest request)
    {
      // Check duplicate email
      var existing = await _userRepo.GetByEmailAsync(request.Email);
      if(existing!=null)
        return (false, "Email already registered");

      // Check duplicate username
      var existingUsername = await _userRepo.GetByUsernameAsync(request.Username);
      if(existingUsername!=null)
        return (false, "Username already taken");

      // Hash the password — NEVER store plain text
      var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

      var user = new User
      {
        Username = request.Username,
        Email = request.Email,
        Password = hashedPassword,
        CreatedAt = DateTime.UtcNow
      };

      await _userRepo.CreateAsync(user);
      return (true, "Registration successful");
    }

    public async Task<(bool success, string message, AuthResponse? data)> LoginAsync(LoginRequest request)
    {
      var user = await _userRepo.GetByEmailAsync(request.Email);

      if(user==null)
        return (false, "Invalid credentials", null);

      // BCrypt compares plain password against stored hash
      bool passwordValid = BCrypt.Net.BCrypt.Verify(request.password, user.Password);

      if(!passwordValid)
        return (false, "Invalid credentials", null);

      var token = _jwtHelper.GenerateToken(user);

      return (true, "Login successful", new AuthResponse
      {
        Token=token,
        Username=user.Username,
        UserId=user.Id
      });

    }
    }
}
