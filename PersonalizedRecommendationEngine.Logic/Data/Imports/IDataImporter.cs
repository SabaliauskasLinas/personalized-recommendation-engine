namespace PersonalizedRecommendationEngine.Logic.Data.Imports
{
    public interface IDataImporter
    {
        void ImportPlayers(string filePath);
        void ImportBets(string filePath);
        void ImportOfferings(string filePath);
    }
}
