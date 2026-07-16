using TaskTracker.Models.Entities;

namespace TaskTracker.Interfaces
{
  public interface IUserRepository
  {
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
  }
}