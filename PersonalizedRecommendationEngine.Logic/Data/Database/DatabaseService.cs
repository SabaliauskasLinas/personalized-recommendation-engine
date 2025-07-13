using Microsoft.Data.Sqlite;

namespace PersonalizedRecommendationEngine.Logic.Data.Database
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _dbPath;

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath ?? throw new ArgumentNullException(nameof(dbPath));
        }

        public SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            return connection;
        }
    }
}
