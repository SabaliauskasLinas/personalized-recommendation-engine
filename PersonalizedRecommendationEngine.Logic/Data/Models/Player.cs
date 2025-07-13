using Microsoft.ML.Data;

namespace PersonalizedRecommendationEngine.Logic.Data.Models
{
    public class Player
    {
        [LoadColumn(0)]
        public long Id { get; set; }
        [LoadColumn(1)]
        public int Age { get; set; }
        [LoadColumn(2)]
        public string Country { get; set; }
        [LoadColumn(3)]
        public string Gender { get; set; }
        [LoadColumn(4)]
        public DateTime RegistrationDate { get; set; }
    }
}
