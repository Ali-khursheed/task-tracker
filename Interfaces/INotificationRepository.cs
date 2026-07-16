using TaskTracker.Models.Entities;

namespace TaskTracker.Interfaces
{
  public interface INotificationRepository
  {
    Task CreateAsync(int userId, int taskId, string message);
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task<int> GetUnreadCountAsync(int userId);
  }
}