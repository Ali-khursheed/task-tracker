using TaskTracker.Models.Dtos.Tasks;
using TaskTracker.Models.Entities;
using TaskTracker.Repositories;
using TaskTrackerAPI.Helpers;

namespace TaskTracker.Services
{
  public class TaskService
  {
    private readonly TaskRepository _taskRepo;
    private readonly NotificationRepository _notifRepo;

    public TaskService(TaskRepository taskRepo, NotificationRepository notifRepo)
    {
      _taskRepo=taskRepo;
      _notifRepo=notifRepo;
    }

    public async Task<int> CreateTaskAsync(CreateTaskRequest request, int assignerId)
    {
      var task = new TaskItem
      {
        Title = request.Title,
        Description = request.Description,
        Assigner_Id = assignerId,
        Assignee_Id = request.Assignee_Id,
        DueDate = request.DueDate,  
        Status = TaskStatusDedo.Assigned,
        CreatedAt = DateTime.UtcNow
      };

      return await _taskRepo.CreateAsync(task);
    }


    public async Task<(bool success, string message)> UpdateStatusAsync(int taskId, string newStatus, int requestingUserID)
    {
      var task=await _taskRepo.GetRawByIdAsync(taskId);
      if(task==null)
      {
        return(false, "Task not found.");
      }
      // Authorization check — who can do what
      if(newStatus==TaskStatusDedo.Completed)
      {
        if(task.Assignee_Id!=requestingUserID)
        {
                   return(false, "Only the assignee can mark the task as completed.");
        }
      }

    else if(newStatus==TaskStatusDedo.Approved)
      {
        // Only assigner can approve
        if(task.Assigner_Id!=requestingUserID)
          return (false, "Only the assigner can approve this task");
      }

    else if(newStatus==TaskStatusDedo.InProgress)
      {
        // Only assignee can start progress
        if(task.Assignee_Id!=requestingUserID)
          return (false, "Only the assignee can update this task");
      }


      // State machine check — is this transition allowed?
      if(!TaskStatusDedo.CanTransition(task.Status, newStatus))
        return (false, $"Cannot transition from {task.Status} to {newStatus}");

      // Set timestamps
      DateTime? completedAt = newStatus == TaskStatusDedo.Completed
                ? DateTime.UtcNow : task.CompletedAt;
      DateTime? approvedAt = newStatus == TaskStatusDedo.Approved
                ? DateTime.UtcNow : task.ApprovedAt;

      await _taskRepo.UpdateStatusAsync(taskId, newStatus, completedAt, approvedAt);

      // Trigger notifications
      if(newStatus==TaskStatusDedo.Completed)
      {
        // Notify assigner that work is done
        await _notifRepo.CreateAsync(
            task.Assigner_Id,
            taskId,
            $"Task '{task.Title}' has been marked as completed by assignee."
        );
      }

      else if(newStatus==TaskStatusDedo.Approved)
      {
        // Notify assignee that work is approved
        await _notifRepo.CreateAsync(
            task.Assignee_Id,
            taskId,
            $"Your work on task '{task.Title}' has been approved!"
        );
      }

      return (true, $"Task status updated to {newStatus}");
    }




    public async Task<IEnumerable<TaskResponse>> GetAssignedToMeAsync(int userId)
            => await _taskRepo.GetAssignedToMeAsync(userId);

    public async Task<IEnumerable<TaskResponse>> GetAssignedByMeAsync(int userId)
        => await _taskRepo.GetAssignedByMeAsync(userId);

    public async Task<TaskResponse?> GetByIdAsync(int id)
        => await _taskRepo.GetByIdAsync(id);






  }
}






