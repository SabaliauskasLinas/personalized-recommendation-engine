using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Services
{
    public interface IRecommendationService
    {
        List<RecommendationResponse> GetRecommendations(int playerId, int topK = 5);
    }
}