using TaskTracker.Models.Dtos.Tasks;

namespace TaskTracker.Interfaces
{
  public interface ITaskService
  {
    Task<int> CreateTaskAsync(CreateTaskRequest request, int assignerId);
    Task<(bool success, string message)> UpdateStatusAsync(
        int taskId, string newStatus, int requestingUserId);
    Task<IEnumerable<TaskResponse>> GetAssignedToMeAsync(int userId);
    Task<IEnumerable<TaskResponse>> GetAssignedByMeAsync(int userId);
    Task<TaskResponse?> GetByIdAsync(int id);
  }
}