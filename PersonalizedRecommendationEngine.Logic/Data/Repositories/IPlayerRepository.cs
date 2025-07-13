using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Data.Repositories
{
    public interface IPlayerRepository
    {
        Player GetPlayerById(int playerId);
    }
}