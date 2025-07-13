namespace PersonalizedRecommendationEngine.Logic.Data.Models
{
    public class RecommendationResponse
    {
        public long OfferId { get; set; }
        public string Sport { get; set; }
        public string Tournament { get; set; }
        public string Team { get; set; }
        public float OddsAmount { get; set; }
        public float PredictedScore { get; set; }
    }
}
