using PersonalizedRecommendationEngine.Logic.Data.Models;

namespace PersonalizedRecommendationEngine.Logic.Data.Repositories
{
    public interface IOfferingRepository
    {
        List<Offering> GetTodayOfferings();
    }
}