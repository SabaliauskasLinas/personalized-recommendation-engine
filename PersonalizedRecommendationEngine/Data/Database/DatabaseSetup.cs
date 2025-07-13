using Microsoft.Data.Sqlite;

namespace PersonalizedRecommendationEngine.Data.Database
{
    public class DatabaseSetup
    {
        private readonly string _dbPath;

        public DatabaseSetup(string dbPath)
        {
            _dbPath = dbPath;
        }

        public void CreateTables()
        {
            var directory = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Players (
                Id INTEGER PRIMARY KEY,
                Age INTEGER,
                Country TEXT,
                Gender TEXT,
                RegistrationDate TEXT
            );

            CREATE TABLE IF NOT EXISTS Bets (
                Id INTEGER PRIMARY KEY,
                PlayerId INTEGER,
                Sport TEXT,
                Tournament TEXT,
                Team TEXT,
                Selection TEXT,
                BetAmount REAL,
                OddsAmount REAL,
                BetDate TEXT,
                FOREIGN KEY (PlayerId) REFERENCES Players(Id)
            );

            CREATE TABLE IF NOT EXISTS Offerings (
                Id INTEGER PRIMARY KEY,
                Sport TEXT,
                Tournament TEXT,
                Team TEXT,
                Selection TEXT,
                OddsAmount REAL,
                EventDate TEXT
            );";
            command.ExecuteNonQuery();
        }
    }
}
