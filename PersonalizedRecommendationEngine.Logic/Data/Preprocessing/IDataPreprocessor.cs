using Microsoft.ML;

namespace PersonalizedRecommendationEngine.Logic.Data.Preprocessing
{
    public interface IDataPreprocessor
    {
        (IDataView TrainingData, IDataView TestingData) PreprocessData();
        IEstimator<ITransformer> GetPreprocessingPipeline();
    }
}
