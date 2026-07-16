using Dapper;
using TaskTracker.Data;
using TaskTracker.Models.Dtos.Tasks;
using TaskTracker.Models.Entities;
using TaskTracker.Interfaces;
namespace TaskTracker.Repositories
{
  public class TaskRepository : ITaskRepository
  {
    private readonly DbConnectionFactory _db;

    public TaskRepository(DbConnectionFactory db)
    {
      _db=db;
    }

    public async Task<int> CreateAsync(TaskItem task)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                INSERT INTO Tasks (Title, Description, Assigner_Id, Assigned_Id, Status, DueDate, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@Title, @Description, @Assigner_ID, @Assigned_ID, @Status, @DueDate, @CreatedAt)";

      return await conn.ExecuteScalarAsync<int>(sql, task);
    }

    public async Task<TaskResponse?> GetByIdAsync(int id)
    {
      using var conn = _db.CreateConnection();

      // JOIN — this is why Dapper is powerful, you write real SQL
      // We get task + both usernames in one query
      var sql = @"
                SELECT 
                    t.Id, t.Title, t.Description, t.Status,
                    t.DueDate, t.CreatedAt, t.Assigner_ID, t.Assigned_ID,
                    u1.Username AS AssignerUsername,
                    u2.Username AS AssigneeUsername
                FROM Tasks t
                INNER JOIN Users u1 ON t.Assigner_ID = u1.Id
                INNER JOIN Users u2 ON t.Assigned_ID = u2.Id
                WHERE t.Id = @Id";

      return await conn.QueryFirstOrDefaultAsync<TaskResponse>(sql, new { Id = id });
    }

    // Tasks assigned TO me (I am the assignee)
    public async Task<IEnumerable<TaskResponse>> GetAssignedToMeAsync(int userId)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                SELECT 
                    t.Id, t.Title, t.Description, t.Status,
                    t.DueDate, t.CreatedAt, t.Assigner_ID, t.Assigned_ID,
                    u1.Username AS AssignerUsername,
                    u2.Username AS AssigneeUsername
                FROM Tasks t
                INNER JOIN Users u1 ON t.Assigner_ID = u1.Id
                INNER JOIN Users u2 ON t.Assigned_ID = u2.Id
                WHERE t.Assigned_ID = @UserId
                ORDER BY t.CreatedAt DESC";

      return await conn.QueryAsync<TaskResponse>(sql, new { UserId = userId });
    }

    // Tasks I created (I am the assigner)
    public async Task<IEnumerable<TaskResponse>> GetAssignedByMeAsync(int userId)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                SELECT 
                    t.Id, t.Title, t.Description, t.Status,
                    t.DueDate, t.CreatedAt, t.Assigner_ID, t.Assigned_ID,
                    u1.Username AS AssignerUsername,
                    u2.Username AS AssigneeUsername
                FROM Tasks t
                INNER JOIN Users u1 ON t.Assigner_ID = u1.Id
                INNER JOIN Users u2 ON t.Assigned_ID = u2.Id
                WHERE t.Assigner_ID = @UserId
                ORDER BY t.CreatedAt DESC";

      return await conn.QueryAsync<TaskResponse>(sql, new { UserId = userId });
    }

    public async Task<TaskItem?> GetRawByIdAsync(int id)
    {
      using var conn = _db.CreateConnection();
      return await conn.QueryFirstOrDefaultAsync<TaskItem>(
          "SELECT * FROM Tasks WHERE Id = @Id",
          new { Id = id }
      );
    }

    public async Task UpdateStatusAsync(int taskId, string newStatus, DateTime? completedAt, DateTime? approvedAt)
    {
      using var conn = _db.CreateConnection();
      var sql = @"
                UPDATE Tasks 
                SET Status = @Status,
                    CompletedAt = @CompletedAt,
                    ApprovedAt = @ApprovedAt
                WHERE Id = @TaskId";

      await conn.ExecuteAsync(sql, new
      {
        Status = newStatus,
        CompletedAt = completedAt,
        ApprovedAt = approvedAt,
        TaskId = taskId
      });
    }
  }
}

