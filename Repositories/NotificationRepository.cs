using Dapper;
using TaskTracker.Data;
using TaskTracker.Models.Entities;
namespace TaskTracker.Repositories
{
  public class NotificationRepository
  {
    private readonly DbConnectionFactory _db;

    public NotificationRepository(DbConnectionFactory db)
    {
      _db=db;
    }

    public async Task CreateAsync(int userId, int taskId, string message)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                INSERT INTO Notifications (UserId, TaskId, Message, IsRead, CreatedAt)
                VALUES (@UserId, @TaskId, @Message, 0, @CreatedAt)";

      await conn.ExecuteAsync(sql, new
      {
        UserId = userId,
        TaskId = taskId,
        Message = message,
        CreatedAt = DateTime.UtcNow
      });
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                SELECT * FROM Notifications 
                WHERE UserId = @UserId 
                ORDER BY CreatedAt DESC";

      return await conn.QueryAsync<Notification>(sql, new { UserId = userId });
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
      using var conn = _db.CreateConnection();
      // userId check prevents user A marking user B's notifications as read
      await conn.ExecuteAsync(
          "UPDATE Notifications SET IsRead = 1 WHERE Id = @Id AND UserId = @UserId",
          new { Id = notificationId, UserId = userId }
      );
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
      using var conn = _db.CreateConnection();
      return await conn.ExecuteScalarAsync<int>(
          "SELECT COUNT(*) FROM Notifications WHERE UserId = @UserId AND IsRead = 0",
          new { UserId = userId }
      );
    }
  }
}
