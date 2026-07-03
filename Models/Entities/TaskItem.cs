namespace TaskTracker.Models.Entities
{
  public class TaskItem
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Assigner_Id { get; set; }
    public int Assignee_Id { get; set; }
    public string? Status { get; set; } 
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
  }
}
