using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using PersonalizedRecommendationEngine.Data.Repositories;
using PersonalizedRecommendationEngine.Logic.Data.Database;
using PersonalizedRecommendationEngine.Logic.Data.Imports;
using PersonalizedRecommendationEngine.Logic.Data.Preprocessing;
using PersonalizedRecommendationEngine.Logic.Data.Repositories;
using PersonalizedRecommendationEngine.Logic.Data.Training;
using PersonalizedRecommendationEngine.Logic.Services;

namespace PersonalizedRecommendationEngine.Logic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string dbPath, IConfiguration configuration)
        {
            services.AddSingleton<MLContext>(new MLContext());
            services.AddSingleton<IDatabaseService, DatabaseService>(sp => new DatabaseService(dbPath));
            services.AddSingleton<IDatabaseSetup, DatabaseSetup>();
            services.AddSingleton<IDataImporter, DataImporter>();
            services.AddSingleton<IDataLoader, DataLoader>(sp => new DataLoader(dbPath, sp.GetRequiredService<MLContext>()));
            services.AddSingleton<IDataPreprocessor, DataPreprocessor>(sp => new DataPreprocessor(sp.GetRequiredService<MLContext>(), sp.GetRequiredService<IDataLoader>()));
            services.AddSingleton<IRecommendationModelTrainer, RecommendationModelTrainer>(sp => new RecommendationModelTrainer(sp.GetRequiredService<MLContext>()));
            services.AddSingleton<IPlayerRepository, PlayerRepository>();
            services.AddSingleton<IBetRepository, BetRepository>();
            services.AddSingleton<IOfferingRepository, OfferingRepository>();
            services.AddSingleton<IRecommendationService, RecommendationService>();

            return services;
        }
    }
}
