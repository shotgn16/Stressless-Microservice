using System.Data;
using System.Data.SQLite;

namespace Stressless_Service.Database
{
    public class DBConnectionFactory
    {
        private readonly string _connectionString;

        public DBConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}
