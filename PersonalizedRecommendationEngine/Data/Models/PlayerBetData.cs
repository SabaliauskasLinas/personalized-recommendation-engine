namespace PersonalizedRecommendationEngine.Data.Models
{
    public class PlayerBetData
    {
        public long PlayerId { get; set; }
        public float Age { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Sport { get; set; }
        public string Tournament { get; set; }
        public string Team { get; set; }
        public string Selection { get; set; }
        public float BetAmount { get; set; }
        public float OddsAmount { get; set; }
        public DateTime BetDate { get; set; }
    }
}
