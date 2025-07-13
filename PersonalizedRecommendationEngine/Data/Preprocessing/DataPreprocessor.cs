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

        // Return raw train/test split (no preprocessing applied)
        public (IDataView TrainingData, IDataView TestingData) PreprocessData()
        {
            // Load data
            var playerData = _dataLoader.LoadPlayerData();
            var betData = _dataLoader.LoadBetData();

            // Join player and bet data (simplified for MVP)
            var joinedData = _mlContext.Data.CreateEnumerable<Player>(
                    playerData, reuseRowObject: false)
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
                ).ToList();

            var rawData = _mlContext.Data.LoadFromEnumerable(joinedData);

            // Split raw data 80/20
            var trainTestSplit = _mlContext.Data.TrainTestSplit(rawData, testFraction: 0.2);

            return (trainTestSplit.TrainSet, trainTestSplit.TestSet);
        }

        // Return unfitted preprocessing pipeline (IEstimator)
        public IEstimator<ITransformer> GetPreprocessingPipeline()
        {
            var pipeline = _mlContext.Transforms
                .ReplaceMissingValues(new[] {
                    new InputOutputColumnPair("BetAmount", "BetAmount"),
                    new InputOutputColumnPair("OddsAmount", "OddsAmount")
                }, MissingValueReplacingEstimator.ReplacementMode.Mean)
                .Append(_mlContext.Transforms.CustomMapping<PlayerBetData, TemporalFeatures>(
                    (input, output) =>
                    {
                        output.DayOfWeek = (float)input.BetDate.DayOfWeek;
                        output.RecencyDays = (float)(DateTime.Today - input.BetDate).TotalDays;
                    }, contractName: "TemporalFeaturesMapping"))
                .Append(_mlContext.Transforms.Conversion.ConvertType(
                    outputColumnName: "Age", inputColumnName: "Age", outputKind: DataKind.Single))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(new[] {
                    new InputOutputColumnPair("Country", "Country"),
                    new InputOutputColumnPair("Gender", "Gender"),
                    new InputOutputColumnPair("Sport", "Sport"),
                    new InputOutputColumnPair("Tournament", "Tournament"),
                    new InputOutputColumnPair("Team", "Team"),
                    new InputOutputColumnPair("Selection", "Selection")
                }))
                .Append(_mlContext.Transforms.NormalizeMinMax(new[] {
                    new InputOutputColumnPair("Age", "Age"),
                    new InputOutputColumnPair("BetAmount", "BetAmount"),
                    new InputOutputColumnPair("OddsAmount", "OddsAmount"),
                    new InputOutputColumnPair("RecencyDays", "RecencyDays")
                }));

            return pipeline;
        }
    }
}
