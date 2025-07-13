using Microsoft.Data.Sqlite;
using System.Globalization;

namespace PersonalizedRecommendationEngine.Logic.Data.Imports
{
    public class DataImporter : IDataImporter
    {
        private readonly string _dbPath;
        public DataImporter(string dbPath)
        {
            _dbPath = dbPath;
        }

        public void ImportPlayers(string playersCsvPath)
        {
            if (!File.Exists(playersCsvPath)) return;

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            foreach (var line in File.ReadAllLines(playersCsvPath).Skip(1))
            {
                var parts = line.Split(',');
                command.CommandText = @"
                    INSERT OR IGNORE INTO Players (Id, Age, Country, Gender, RegistrationDate)
                    VALUES ($id, $age, $country, $gender, $regDate)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$id", long.Parse(parts[0]));
                command.Parameters.AddWithValue("$age", int.Parse(parts[1]));
                command.Parameters.AddWithValue("$country", parts[2]);
                command.Parameters.AddWithValue("$gender", parts[3]);
                command.Parameters.AddWithValue("$regDate", parts[4]);
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        public void ImportBets(string betsCsvPath)
        {
            if (!File.Exists(betsCsvPath)) return;

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            foreach (var line in File.ReadAllLines(betsCsvPath).Skip(1))
            {
                var parts = line.Split(',');
                command.CommandText = @"
                    INSERT OR IGNORE INTO Bets (Id, PlayerId, Sport, Tournament, Team, Selection, BetAmount, OddsAmount, BetDate)
                    VALUES ($id, $playerId, $sport, $tournament, $team, $selection, $betAmount, $oddsAmount, $betDate)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$id", long.Parse(parts[0]));
                command.Parameters.AddWithValue("$playerId", long.Parse(parts[1]));
                command.Parameters.AddWithValue("$sport", parts[2]);
                command.Parameters.AddWithValue("$tournament", parts[3]);
                command.Parameters.AddWithValue("$team", parts[4]);
                command.Parameters.AddWithValue("$selection", parts[5]);
                command.Parameters.AddWithValue("$betAmount", decimal.Parse(parts[6], CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("$oddsAmount", decimal.Parse(parts[7], CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("$betDate", parts[8]);
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        public void ImportOfferings(string offeringsCsvPath)
        {
            if (!File.Exists(offeringsCsvPath)) return;

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var command = connection.CreateCommand();
            foreach (var line in File.ReadAllLines(offeringsCsvPath).Skip(1))
            {
                var parts = line.Split(',');
                command.CommandText = @"
                    INSERT OR IGNORE INTO Offerings (Id, Sport, Tournament, Team, Selection, OddsAmount, EventDate)
                    VALUES ($id, $sport, $tournament, $team, $selection, $oddsAmount, $eventDate)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("$id", long.Parse(parts[0]));
                command.Parameters.AddWithValue("$sport", parts[1]);
                command.Parameters.AddWithValue("$tournament", parts[2]);
                command.Parameters.AddWithValue("$team", parts[3]);
                command.Parameters.AddWithValue("$selection", parts[4]);
command.Parameters.AddWithValue("$oddsAmount", decimal.Parse(parts[5], CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("$eventDate", parts[6]);
                command.ExecuteNonQuery();
            }
            transaction.Commit();
        }
    }
}