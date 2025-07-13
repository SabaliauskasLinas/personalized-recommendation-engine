using Microsoft.Data.Sqlite;

namespace PersonalizedRecommendationEngine.Logic.Data.Database
{
    public interface IDatabaseService
    {
        SqliteConnection GetConnection();
    }
}