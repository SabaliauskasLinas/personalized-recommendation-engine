using PersonalizedRecommendationEngine.Logic.Data.Database;
using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Data.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly IDatabaseService _databaseService;

        public BetRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public List<Bet> GetBetsByPlayerId(int playerId)
        {
            var bets = new List<Bet>();
            using var connection = _databaseService.GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, PlayerId, Sport, Tournament, Team, Selection, BetAmount, OddsAmount, BetDate FROM Bets WHERE PlayerId = $id";
            command.Parameters.AddWithValue("$id", playerId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                bets.Add(new Bet
                {
                    Id = reader.GetInt64(0),
                    PlayerId = reader.GetInt32(1),
                    Sport = reader.GetString(2),
                    Tournament = reader.GetString(3),
                    Team = reader.GetString(4),
                    Selection = reader.GetString(5),
                    BetAmount = reader.GetFloat(6),
                    OddsAmount = reader.GetFloat(7),
                    BetDate = reader.GetDateTime(8)
                });
            }
            return bets;
        }
    }
}
