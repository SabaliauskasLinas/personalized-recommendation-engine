namespace PersonalizedRecommendationEngine.Logic.Data.Models
{
    public class RecommendationInput
    {
        public uint PlayerId { get; set; }
        public uint ItemId { get; set; }
        public float Label { get; set; }
    }
}
