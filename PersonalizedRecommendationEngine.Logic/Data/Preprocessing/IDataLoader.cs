using Microsoft.ML;

namespace PersonalizedRecommendationEngine.Logic.Data.Preprocessing
{
    public interface IDataLoader
    {
        IDataView LoadPlayerData();
        IDataView LoadBetData();
    }
}
