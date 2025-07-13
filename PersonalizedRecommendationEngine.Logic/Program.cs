using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;
using PersonalizedRecommendationEngine.Logic;
using PersonalizedRecommendationEngine.Logic.Data.Database;
using PersonalizedRecommendationEngine.Logic.Data.Imports;
using PersonalizedRecommendationEngine.Logic.Data.Preprocessing;
using PersonalizedRecommendationEngine.Logic.Data.Training;

namespace PersonalizedRecommendationEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Database:Path"] = "Data/Database/betting_data.db",
                    ["Model:Path"] = "Data/Training/model.onnx"
                })
                .Build();

            services.AddServices("Data/Database/betting_data.db", configuration);

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                // Initialize database
                var dbSetup = serviceProvider.GetRequiredService<IDatabaseSetup>();
                dbSetup.CreateTables();
                Console.WriteLine("Database tables created.");

                // Import data
                var importer = serviceProvider.GetRequiredService<IDataImporter>();
                importer.ImportPlayers("Data/Imports/Csv/players.csv");
                Console.WriteLine("Players import completed.");
                importer.ImportBets("Data/Imports/Csv/bets.csv");
                Console.WriteLine("Bets import completed.");
                importer.ImportOfferings("Data/Imports/Csv/offerings.csv");
                Console.WriteLine("Offerings import completed.");

                // Preprocess data (returns raw train/test split without transformation)
                var mlContext = serviceProvider.GetRequiredService<MLContext>();
                var preprocessor = serviceProvider.GetRequiredService<IDataPreprocessor>();
                var (trainingData, testingData) = preprocessor.PreprocessData();
                Console.WriteLine("Data split completed.");

                // Count rows in training and testing data
                long trainingCount = mlContext.Data.CreateEnumerable<object>(
                    trainingData, reuseRowObject: false, ignoreMissingColumns: true).Count();
                long testingCount = mlContext.Data.CreateEnumerable<object>(
                    testingData, reuseRowObject: false, ignoreMissingColumns: true).Count();
                Console.WriteLine($"Training data rows: {trainingCount}");
                Console.WriteLine($"Testing data rows: {testingCount}");

                // Train and evaluate model using raw training/testing data
                var trainer = serviceProvider.GetRequiredService<IRecommendationModelTrainer>();
                var (model, inputSchema) = trainer.TrainModel(trainingData);
                Console.WriteLine("Model training completed.");

                trainer.EvaluateModel(model, testingData);
                Console.WriteLine("Model evaluation completed.");

                // Save model - create directory if needed
                var modelPath = "Data/Training/model.onnx";
                var modelDirectory = Path.GetDirectoryName(modelPath);
                if (!Directory.Exists(modelDirectory) && !string.IsNullOrEmpty(modelDirectory))
                {
                    Directory.CreateDirectory(modelDirectory);
                }

                mlContext.Model.Save(model, inputSchema, modelPath);
                Console.WriteLine($"Model saved to {modelPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}