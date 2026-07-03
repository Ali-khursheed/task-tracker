using Microsoft.Data.SqlClient;
using System.Data;
namespace TaskTracker.Data
{
  public class DbConnectionFactory
  {
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuaration)
    {
      _connectionString=configuaration.GetConnectionString("DefaultConnection")!;
    }



    public IDbConnection CreateConnection()
    {
      return new SqlConnection(_connectionString);
    }
  }




}


  
