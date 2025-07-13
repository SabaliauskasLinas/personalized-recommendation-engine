using Microsoft.ML.Data;

namespace PersonalizedRecommendationEngine.Data.Models
{
    public class RecommendationPrediction
    {
        [ColumnName("Score")]
        public float Score { get; set; } // Predicted preference score
    }
}
