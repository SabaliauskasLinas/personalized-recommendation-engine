using Microsoft.ML;
using PersonalizedRecommendationEngine.Data.Database;
using PersonalizedRecommendationEngine.Data.Imports;
using PersonalizedRecommendationEngine.Data.Preprocessing;
using PersonalizedRecommendationEngine.Data.Training;
using System;
using System.IO;
using System.Linq;

namespace PersonalizedRecommendationEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define paths
            string dbPath = "Data/Database/betting_data.db";
            string playersCsvPath = "Data/Imports/Csv/players.csv";
            string betsCsvPath = "Data/Imports/Csv/bets.csv";
            string offeringsCsvPath = "Data/Imports/Csv/offerings.csv";
            string modelPath = "Data/Training/model.onnx";

            try
            {
                // Initialize database
                var dbSetup = new DatabaseSetup(dbPath);
                dbSetup.CreateTables();
                Console.WriteLine("Database tables created.");

                // Import data
                var importer = new DataImporter(dbPath);
                importer.ImportPlayers(playersCsvPath);
                Console.WriteLine("Players import completed.");
                importer.ImportBets(betsCsvPath);
                Console.WriteLine("Bets import completed.");
                importer.ImportOfferings(offeringsCsvPath);
                Console.WriteLine("Offerings import completed.");

                // Preprocess data (returns raw train/test split without transformation)
                var mlContext = new MLContext();
                var dataLoader = new DataLoader(dbPath, mlContext);
                var preprocessor = new DataPreprocessor(mlContext, dataLoader);
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
                var trainer = new RecommendationModelTrainer(mlContext, preprocessor);
                var (model, inputSchema) = trainer.TrainModel(trainingData);
                Console.WriteLine("Model training completed.");

                trainer.EvaluateModel(model, testingData);
                Console.WriteLine("Model evaluation completed.");

                // Save model - create directory if needed
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