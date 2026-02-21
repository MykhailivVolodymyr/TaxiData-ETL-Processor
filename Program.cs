using TaxiData_ETL_Processor.Services;

class Program
{
    private static readonly string ConnectionString = @"Server=DESKTOP-1BTCT19\SQLSERVER2;Database=TaxiDb;Trusted_Connection=True;TrustServerCertificate=True;";

    static void Main(string[] args)
    {
        var parser = new TaxiParser();
        var db = new DatabaseService(ConnectionString);

        try
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));
            string inputPath = Path.Combine(projectRoot, "Data", "sample-cab-data.csv");
            string duplicatesPath = Path.Combine(projectRoot, "Data", "duplicates.csv");

            // 1. Execute ETL
            var (valid, duplicates) = parser.ParseCsv(inputPath);

            // 2. Handle Duplicates
            if (duplicates.Any())
            {
                parser.SaveDuplicatesToFile(duplicates, duplicatesPath);
                Console.WriteLine($"Found and saved {duplicates.Count} duplicates to CSV.");
            }

            // 3. Handle Database Import
            if (valid.Any())
            {
                Console.WriteLine($"Importing {valid.Count} valid records to SQL...");
                db.BulkInsert(valid);
                Console.WriteLine("Database import completed successfully!");
            }
            else
            {
                Console.WriteLine("No valid records found to import.");
            }

            Console.WriteLine("Process finished!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Error: {ex.Message}");
        }
    }
}