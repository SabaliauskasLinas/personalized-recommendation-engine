using PersonalizedRecommendationEngine.Logic.Data.Database;
using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Data.Repositories
{
    public class OfferingRepository : IOfferingRepository
    {
        private readonly IDatabaseService _databaseService;

        public OfferingRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        }

        public List<Offering> GetTodayOfferings()
        {
            var offerings = new List<Offering>();
            using var connection = _databaseService.GetConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Sport, Tournament, Team, OddsAmount, EventDate FROM Offerings WHERE EventDate >= $today AND EventDate < $tomorrow";
            command.Parameters.AddWithValue("$today", DateTime.Today);
            command.Parameters.AddWithValue("$tomorrow", DateTime.Today.AddDays(1));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                offerings.Add(new Offering
                {
                    Id = reader.GetInt64(0),
                    Sport = reader.GetString(1),
                    Tournament = reader.GetString(2),
                    Team = reader.GetString(3),
                    OddsAmount = reader.GetFloat(4),
                    EventDate = reader.GetDateTime(5)
                });
            }
            return offerings;
        }
    }
}
