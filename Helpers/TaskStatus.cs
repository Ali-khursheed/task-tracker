namespace TaskTrackerAPI.Helpers
{
  public static class TaskStatusDedo
  {
    public const string Assigned = "Assigned";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Approved = "Approved";

    // State machine rules — what transition is allowed from each state
    public static readonly Dictionary<string, List<string>> AllowedTransitions = new()
        {
            { Assigned,   new List<string> { InProgress, Completed } },
            { InProgress, new List<string> { Completed } },
            { Completed,  new List<string> { Approved } },
            { Approved,   new List<string>() }  // terminal state, nothing allowed
        };

    public static bool CanTransition(string currentStatus, string newStatus)
    {
      if(!AllowedTransitions.ContainsKey(currentStatus))
        return false;

      return AllowedTransitions[currentStatus].Contains(newStatus);
    }
  }
}