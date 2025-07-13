using Microsoft.ML;
using PersonalizedRecommendationEngine.Data.Models;
using PersonalizedRecommendationEngine.Data.Preprocessing;

namespace PersonalizedRecommendationEngine.Data.Training
{
    public class RecommendationModelTrainer
    {
        private readonly MLContext _mlContext;
        private readonly DataPreprocessor _preprocessor;

        public RecommendationModelTrainer(MLContext mlContext, DataPreprocessor preprocessor)
        {
            _mlContext = mlContext;
            _preprocessor = preprocessor;
        }

        public (ITransformer Model, DataViewSchema InputSchema) TrainModel(IDataView trainingData)
        {
            // Convert preprocessed data to recommendation input
            var recommendationData = _mlContext.Data.CreateEnumerable<PlayerBetData>(
                trainingData, reuseRowObject: false)
                .Select(d => new RecommendationInput
                {
                    PlayerId = (uint)d.PlayerId,
                    ItemId = (uint)(d.Sport + d.Tournament + d.Team + d.Selection).GetHashCode(),
                    Label = 1.0f // Simplified: 1 for bets placed
                }).ToList();

            var recommendationDataView = _mlContext.Data.LoadFromEnumerable(recommendationData);

            // Define matrix factorization pipeline
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "PlayerIdEncoded", inputColumnName: "PlayerId")
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "ItemIdEncoded", inputColumnName: "ItemId"))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: "Label",
                    matrixColumnIndexColumnName: "PlayerIdEncoded",
                    matrixRowIndexColumnName: "ItemIdEncoded",
                    numberOfIterations: 20,
                    approximationRank: 100));

            // Train model
            var model = pipeline.Fit(recommendationDataView);

            return (model, recommendationDataView.Schema);
        }

        public void EvaluateModel(ITransformer model, IDataView testData)
        {
            // Convert test data to recommendation input
            var testRecommendationData = _mlContext.Data.CreateEnumerable<PlayerBetData>(
                testData, reuseRowObject: false)
                .Select(d => new RecommendationInput
                {
                    PlayerId = (uint)d.PlayerId,
                    ItemId = (uint)(d.Sport + d.Tournament + d.Team + d.Selection).GetHashCode(),
                    Label = 1.0f
                }).ToList();

            var testDataView = _mlContext.Data.LoadFromEnumerable(testRecommendationData);

            // Evaluate model
            var predictions = model.Transform(testDataView);
            var metrics = _mlContext.Recommendation().Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine($"RMSE: {metrics.MeanSquaredError:F4}");
            Console.WriteLine($"Loss Function: {metrics.LossFunction:F4}");
        }
    }
}
