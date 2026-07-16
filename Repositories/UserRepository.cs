using Dapper;
using TaskTracker.Data;
using TaskTracker.Interfaces;
using TaskTracker.Models.Entities;


namespace TaskTracker.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly DbConnectionFactory _db;

    public UserRepository(DbConnectionFactory db)
    {
      _db=db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
      using var conn = _db.CreateConnection();
      // Dapper maps SQL result columns to User properties automatically
      return await conn.QueryFirstOrDefaultAsync<User>(
          "SELECT * FROM Users WHERE Email = @Email",
          new { Email = email }
      );
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
      using var conn = _db.CreateConnection();
      return await conn.QueryFirstOrDefaultAsync<User>(
          "SELECT * FROM Users WHERE Username = @Username",
          new { Username = username }
      );
    }


    public async Task<int> CreateAsync(User user)
    {
      using var conn = _db.CreateConnection();
      // OUTPUT INSERTED.Id returns the new row's Id
      var sql = @"
                INSERT INTO Users (Username, Email, Password, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@Username, @Email, @Password, @CreatedAt)";

      return await conn.ExecuteScalarAsync<int>(sql, user);
    }


    public async Task<IEnumerable<User>> GetAllAsync()
    {
      using var conn = _db.CreateConnection();
      return await conn.QueryAsync<User>("SELECT Id, Username, Email FROM Users");
    }
  }
}
