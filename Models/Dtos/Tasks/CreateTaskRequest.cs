namespace TaskTracker.Models.Dtos.Tasks
{
  public class CreateTaskRequest
  {
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Assignee_Id { get; set; }
    public DateTime? DueDate { get; set; }
  }
}
