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
            services.AddSingleton<IRecommendationModelTrainer, RecommendationModelTrainer>();
            services.AddTransient<IDataImporter, DataImporter>();
            services.AddScoped<IDataLoader, DataLoader>(sp => new DataLoader(dbPath, sp.GetRequiredService<MLContext>()));
            services.AddScoped<IDataPreprocessor, DataPreprocessor>(sp => new DataPreprocessor(sp.GetRequiredService<MLContext>(), sp.GetRequiredService<IDataLoader>()));
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IBetRepository, BetRepository>();
            services.AddScoped<IOfferingRepository, OfferingRepository>();
            services.AddScoped<IRecommendationService, RecommendationService>();

            return services;
        }
    }
}
