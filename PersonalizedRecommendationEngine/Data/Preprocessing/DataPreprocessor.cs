using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using PersonalizedRecommendationEngine.Data.Models;

namespace PersonalizedRecommendationEngine.Data.Preprocessing
{
    public class DataPreprocessor
    {
        private readonly MLContext _mlContext;
        private readonly DataLoader _dataLoader;

        public DataPreprocessor(MLContext mlContext, DataLoader dataLoader)
        {
            _mlContext = mlContext;
            _dataLoader = dataLoader;
        }

        public (IDataView TrainingData, IDataView TestingData, ITransformer PreprocessingPipeline) PreprocessData()
        {
            // Load data
            var playerData = _dataLoader.LoadPlayerData();
            var betData = _dataLoader.LoadBetData();

            // Join player and bet data (simplified for MVP)
            var joinedData = _mlContext.Data.CreateEnumerable<PlayerBetData>(
                _mlContext.Data.LoadFromEnumerable(
                    _mlContext.Data.CreateEnumerable<Player>(playerData, reuseRowObject: false)
                        .Join(
                            _mlContext.Data.CreateEnumerable<Bet>(betData, reuseRowObject: false),
                            p => p.Id,
                            b => b.PlayerId,
                            (p, b) => new PlayerBetData
                            {
                                PlayerId = p.Id,
                                Age = p.Age,
                                Country = p.Country,
                                Gender = p.Gender,
                                RegistrationDate = p.RegistrationDate,
                                Sport = b.Sport,
                                Tournament = b.Tournament,
                                Team = b.Team,
                                Selection = b.Selection,
                                BetAmount = b.BetAmount,
                                OddsAmount = b.OddsAmount,
                                BetDate = b.BetDate
                            }
                        )
                ),
                reuseRowObject: false
            ).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(joinedData);

            // Define preprocessing pipeline
            var pipeline = _mlContext.Transforms
                // Handle missing values
                .ReplaceMissingValues(new[] {
                    new InputOutputColumnPair("BetAmount", "BetAmount"),
                    new InputOutputColumnPair("OddsAmount", "OddsAmount")
                }, MissingValueReplacingEstimator.ReplacementMode.Mean)
                // Extract temporal features: add DayOfWeek and RecencyDays as new columns
                .Append(_mlContext.Transforms.CustomMapping<PlayerBetData, TemporalFeatures>(
                    (input, output) =>
                    {
                        output.DayOfWeek = (float)input.BetDate.DayOfWeek;
                        output.RecencyDays = (float)(DateTime.Today - input.BetDate).TotalDays;
                    }, contractName: "TemporalFeaturesMapping"))
                // Convert Age from int to float
                .Append(_mlContext.Transforms.Conversion.ConvertType(
                    outputColumnName: "Age", inputColumnName: "Age", outputKind: DataKind.Single))
                // Encode categorical features
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(new[] {
                    new InputOutputColumnPair("Country", "Country"),
                    new InputOutputColumnPair("Gender", "Gender"),
                    new InputOutputColumnPair("Sport", "Sport"),
                    new InputOutputColumnPair("Tournament", "Tournament"),
                    new InputOutputColumnPair("Team", "Team"),
                    new InputOutputColumnPair("Selection", "Selection")
                }))
                // Normalize numeric features
                .Append(_mlContext.Transforms.NormalizeMinMax(new[] {
                    new InputOutputColumnPair("Age", "Age"),
                    new InputOutputColumnPair("BetAmount", "BetAmount"),
                    new InputOutputColumnPair("OddsAmount", "OddsAmount"),
                    new InputOutputColumnPair("RecencyDays", "RecencyDays")
                }));

            // Fit pipeline
            var preprocessingPipeline = pipeline.Fit(dataView);
            var transformedData = preprocessingPipeline.Transform(dataView);

            // Split data: 80% training, 20% testing
            var trainTestSplit = _mlContext.Data.TrainTestSplit(transformedData, testFraction: 0.2);

            return (trainTestSplit.TrainSet, trainTestSplit.TestSet, preprocessingPipeline);
        }
    }
}
