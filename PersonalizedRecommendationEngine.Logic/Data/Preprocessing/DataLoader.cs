using Microsoft.Data.Sqlite;
using Microsoft.ML;
using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Data.Preprocessing
{
    public class DataLoader : IDataLoader
    {
        private readonly string _dbPath;
        private readonly MLContext _mlContext;

        public DataLoader(string dbPath, MLContext mlContext)
        {
            _dbPath = dbPath;
            _mlContext = mlContext;
        }

        public IDataView LoadPlayerData()
        {
            var players = new List<Player>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Age, Country, Gender, RegistrationDate FROM Players";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                players.Add(new Player
                {
                    Id = reader.GetInt64(0),
                    Age = reader.GetInt32(1),
                    Country = reader.GetString(2),
                    Gender = reader.GetString(3),
                    RegistrationDate = DateTime.Parse(reader.GetString(4))
                });
            }

            return _mlContext.Data.LoadFromEnumerable(players);
        }

        public IDataView LoadBetData()
        {
            var bets = new List<Bet>();
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, PlayerId, Sport, Tournament, Team, Selection, BetAmount, OddsAmount, BetDate FROM Bets";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                bets.Add(new Bet
                {
                    Id = reader.GetInt64(0),
                    PlayerId = reader.GetInt64(1),
                    Sport = reader.GetString(2),
                    Tournament = reader.GetString(3),
                    Team = reader.GetString(4),
                    Selection = reader.GetString(5),
                    BetAmount = reader.GetFloat(6),
                    OddsAmount = reader.GetFloat(7),
                    BetDate = DateTime.Parse(reader.GetString(8))
                });
            }

            return _mlContext.Data.LoadFromEnumerable(bets);
        }
    }
}
