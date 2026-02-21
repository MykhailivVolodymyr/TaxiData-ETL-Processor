# NYC Taxi Data ETL Processor

## Project Structure
- `Services/`: Contains core logic.
  - `TaxiParser.cs`: Handles CSV parsing, data transformation, and duplicate detection.
  - `DatabaseService.cs`: Manages high-speed SQL Server bulk inserts.
- `Database/`: Contains database scripts.
  - `init.sql`: SQL script to create the table and optimized indexes.
- `Data/`: Directory for source and output files.
  - `sample-cab-data.csv`: Input data file.
  - `duplicates.csv`: Output file with identified duplicates.
- `Mappings/`: Contains CsvHelper class maps.
- `Models/`: Data transfer objects (DTOs).

## How to Run
1. Update the `ConnectionString` in `Program.cs` with your local SQL Server details.
2. Run the provided SQL script to create the table and indexes.
3. Place the `sample-cab-data.csv` in the `Data/` folder. Link here https://drive.google.com/file/d/1l2ARvh1-tJBqzomww45TrGtIh5j8Vud4/view?usp=sharing.
4. Execute the application.
