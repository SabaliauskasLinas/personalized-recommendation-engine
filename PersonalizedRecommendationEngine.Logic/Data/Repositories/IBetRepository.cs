using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Data.Repositories
{
    public interface IBetRepository
    {
        List<Bet> GetBetsByPlayerId(int playerId);
    }
}