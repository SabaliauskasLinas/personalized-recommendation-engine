using Microsoft.ML;
using PersonalizedRecommendationEngine.Data.Database;
using PersonalizedRecommendationEngine.Data.Imports;
using PersonalizedRecommendationEngine.Data.Preprocessing;

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

                // Preprocess data
                var mlContext = new MLContext();
                var dataLoader = new DataLoader(dbPath, mlContext);
                var preprocessor = new DataPreprocessor(mlContext, dataLoader);
                var (trainingData, testingData, preprocessingPipeline) = preprocessor.PreprocessData();
                Console.WriteLine("Data preprocessing completed.");

                // Count rows in training and testing data
                long trainingCount = mlContext.Data.CreateEnumerable<object>(
                    trainingData, reuseRowObject: false, ignoreMissingColumns: true).Count();
                long testingCount = mlContext.Data.CreateEnumerable<object>(
                    testingData, reuseRowObject: false, ignoreMissingColumns: true).Count();
                Console.WriteLine($"Training data rows: {trainingCount}");
                Console.WriteLine($"Testing data rows: {testingCount}");

                // Save preprocessing pipeline
                Directory.CreateDirectory("Data/Preprocessing");
                mlContext.Model.Save(preprocessingPipeline, trainingData.Schema, "Data/Preprocessing/preprocessing_pipeline.zip");
                Console.WriteLine("Preprocessing pipeline saved.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}