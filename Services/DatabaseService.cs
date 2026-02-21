using Microsoft.Data.SqlClient;
using FastMember;
using TaxiData_ETL_Processor.Models;

namespace TaxiData_ETL_Processor.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void BulkInsert(List<TaxiTrip> trips)
    {
        using var bcp = new SqlBulkCopy(_connectionString);
        bcp.DestinationTableName = "TaxiTrips";
        bcp.BatchSize = 10000;

        bcp.ColumnMappings.Add(nameof(TaxiTrip.TpepPickupDatetime), "tpep_pickup_datetime");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.TpepDropoffDatetime), "tpep_dropoff_datetime");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.PassengerCount), "passenger_count");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.TripDistance), "trip_distance");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.StoreAndFwdFlag), "store_and_fwd_flag");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.PULocationID), "PULocationID");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.DOLocationID), "DOLocationID");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.FareAmount), "fare_amount");
        bcp.ColumnMappings.Add(nameof(TaxiTrip.TipAmount), "tip_amount");

        using var reader = ObjectReader.Create(trips);
        bcp.WriteToServer(reader);
    }
}