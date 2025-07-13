namespace PersonalizedRecommendationEngine.Data.Models
{
    public class RecommendationInput
    {
        public uint PlayerId { get; set; } // uint for matrix factorization
        public uint ItemId { get; set; }   // Combined Sport+Tournament+Team+Selection
        public float Label { get; set; }   // Preference score (e.g., 1 for a bet placed)
    }
}
