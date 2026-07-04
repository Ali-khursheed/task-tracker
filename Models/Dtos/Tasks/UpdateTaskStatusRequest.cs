namespace TaskTracker.Models.Dtos.Tasks
{
  public class UpdateTaskStatusRequest
  {
    public string NewStatus { get; set; } = string.Empty;
  }
}
