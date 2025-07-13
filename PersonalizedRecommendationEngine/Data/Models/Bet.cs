using Microsoft.ML.Data;

namespace PersonalizedRecommendationEngine.Data.Models
{
    public class Bet
    {
        [LoadColumn(0)]
        public long Id { get; set; }
        [LoadColumn(1)]
        public long PlayerId { get; set; }
        [LoadColumn(2)]
        public string Sport { get; set; }
        [LoadColumn(3)]
        public string Tournament { get; set; }
        [LoadColumn(4)]
        public string Team { get; set; }
        [LoadColumn(5)]
        public string Selection { get; set; }
        [LoadColumn(6)]
        public float BetAmount { get; set; }
        [LoadColumn(7)]
        public float OddsAmount { get; set; }
        [LoadColumn(8)]
        public DateTime BetDate { get; set; }
    }
}
