namespace TaskTracker.Models.Entities
{
  public class Notification
  {
    public int Id { get; set; }
    public int UserID { get; set; }
    public int TaskID { get; set; }
     public string? Message { get; set; }
    public bool? IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

   
  }
}
