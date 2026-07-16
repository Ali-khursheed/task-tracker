using TaskTracker.Models.Dtos.Tasks;

using TaskTracker.Models.Entities;

namespace TaskTracker.Interfaces
{
  public interface ITaskRepository
  {
    Task<int> CreateAsync(TaskItem task);
    Task<TaskResponse?> GetByIdAsync(int id);
    Task<IEnumerable<TaskResponse>> GetAssignedToMeAsync(int userId);
    Task<IEnumerable<TaskResponse>> GetAssignedByMeAsync(int userId);
    Task<TaskItem?> GetRawByIdAsync(int id);
    Task UpdateStatusAsync(int taskId, string newStatus,
                           DateTime? completedAt, DateTime? approvedAt);
  }
}