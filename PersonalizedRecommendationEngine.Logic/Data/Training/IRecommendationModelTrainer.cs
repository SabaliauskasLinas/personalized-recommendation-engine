using Microsoft.ML;

namespace PersonalizedRecommendationEngine.Logic.Data.Training
{
    public interface IRecommendationModelTrainer
    {
        (ITransformer Model, DataViewSchema InputSchema) TrainModel(IDataView trainingData);
        void EvaluateModel(ITransformer model, IDataView testData);
    }
}
