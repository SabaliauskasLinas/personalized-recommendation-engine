using PersonalizedRecommendationEngine.Logic.Data.Database;
using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Data.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDatabaseService _databaseService;

        public PlayerRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Player GetPlayerById(int playerId)
        {
            using var connection = _databaseService.GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Age, Country, Gender, RegistrationDate FROM Players WHERE Id = $id";
            command.Parameters.AddWithValue("$id", playerId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Player
                {
                    Id = reader.GetInt32(0),
                    Age = reader.GetInt32(1),
                    Country = reader.GetString(2),
                    Gender = reader.GetString(3),
                    RegistrationDate = reader.GetDateTime(4)
                };
            }
            throw new Exception($"Player with ID {playerId} not found.");
        }
    }
}