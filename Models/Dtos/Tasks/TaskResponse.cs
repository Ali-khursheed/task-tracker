namespace TaskTracker.Models.Dtos.Tasks
{
  public class TaskResponse
  {
    public int Id { get; set; }
    public string? Title { get; set; } 
    public string? Description { get; set; }
    public string? Status { get; set; } 
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // We show names, not just IDs — friendlier for frontend
    public string ?AssignerUsername { get; set; }
    public string ?AssigneeUsername { get; set; }
    public int Assigner_ID { get; set; }
    public int Assigned_ID { get; set; }

  }
}
