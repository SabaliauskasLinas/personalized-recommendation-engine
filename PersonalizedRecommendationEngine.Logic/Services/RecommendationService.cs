using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using PersonalizedRecommendationEngine.Data.Repositories;
using PersonalizedRecommendationEngine.Logic.Data.Models;
using PersonalizedRecommendationEngine.Logic.Data.Preprocessing;
using PersonalizedRecommendationEngine.Logic.Data.Repositories;

namespace PersonalizedRecommendationEngine.Logic.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly DataViewSchema _modelSchema;
        private readonly IDataPreprocessor _preprocessor;
        private readonly IPlayerRepository _playerRepository;
        private readonly IBetRepository _betRepository;
        private readonly IOfferingRepository _offeringRepository;

        public RecommendationService(
            MLContext mlContext,
            IDataPreprocessor preprocessor,
            IPlayerRepository playerRepository,
            IBetRepository betRepository,
            IOfferingRepository offeringRepository,
            IConfiguration configuration)
        {
            _mlContext = mlContext;
            _preprocessor = preprocessor;
            _playerRepository = playerRepository;
            _betRepository = betRepository;
            _offeringRepository = offeringRepository;
            _model = _mlContext.Model.Load(configuration["Model:Path"], out _modelSchema);
        }

        public List<RecommendationResponse> GetRecommendations(int playerId, int topK = 5)
        {
            var playerData = _playerRepository.GetPlayerById(playerId);
            var bettingHistory = _betRepository.GetBetsByPlayerId(playerId);
            var todayOfferings = _offeringRepository.GetTodayOfferings();

            var playerBetDataList = bettingHistory.Select(b => new PlayerBetData
            {
                PlayerId = playerId,
                Age = playerData.Age,
                Country = playerData.Country,
                Gender = playerData.Gender,
                RegistrationDate = playerData.RegistrationDate,
                Sport = b.Sport,
                Tournament = b.Tournament,
                Team = b.Team,
                Selection = b.Selection,
                BetAmount = b.BetAmount,
                OddsAmount = b.OddsAmount,
                BetDate = b.BetDate
            }).ToList();

            var dataView = _mlContext.Data.LoadFromEnumerable(playerBetDataList);
            var preprocessingPipeline = _preprocessor.GetPreprocessingPipeline();
            var preprocessedData = preprocessingPipeline.Fit(dataView).Transform(dataView);

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<RecommendationInput, RecommendationOutput>(
                _model, inputSchema: _modelSchema);

            var recommendations = new List<RecommendationResponse>();
            foreach (var offer in todayOfferings)
            {
                var input = new RecommendationInput
                {
                    PlayerId = (uint)playerId,
                    ItemId = (uint)(offer.Sport + offer.Tournament + offer.Team).GetHashCode(),
                    Label = 0f
                };

                var prediction = predictionEngine.Predict(input);
                recommendations.Add(new RecommendationResponse
                {
                    OfferId = offer.Id,
                    Sport = offer.Sport,
                    Tournament = offer.Tournament,
                    Team = offer.Team,
                    OddsAmount = offer.OddsAmount,
                    PredictedScore = prediction.Score
                });
            }

            return recommendations.OrderByDescending(r => r.PredictedScore).Take(topK).ToList();
        }
    }
}
