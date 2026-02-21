using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using TaxiData_ETL_Processor.Mappings;
using TaxiData_ETL_Processor.Models;

namespace TaxiData_ETL_Processor.Services;

public class TaxiParser
{
    private readonly TimeZoneInfo _estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public (List<TaxiTrip> Valid, List<TaxiTrip> Duplicates) ParseCsv(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Input file not found at: {path}");
        }

        var validTrips = new List<TaxiTrip>();
        var duplicateTrips = new List<TaxiTrip>();
        var seenKeys = new HashSet<(DateTime, DateTime, int?)>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null
        };

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<TaxiTripMap>();

        while (csv.Read())
        {
            var record = csv.GetRecord<TaxiTrip>();
            TransformRecord(record);

            // duplicate check
            var key = (record.TpepPickupDatetime, record.TpepDropoffDatetime, record.PassengerCount);

            if (!seenKeys.Add(key))
            {
                duplicateTrips.Add(record);
            }
            else
            {
                validTrips.Add(record);
            }
        }

        return (validTrips, duplicateTrips);
    }

    public void SaveDuplicatesToFile(List<TaxiTrip> duplicates, string path)
    {
        if (duplicates == null || !duplicates.Any())
        {
            return;
        }

        string directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(path);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(duplicates);
    }

    private void TransformRecord(TaxiTrip record)
    {
        record.TpepPickupDatetime = ConvertToUtc(record.TpepPickupDatetime);
        record.TpepDropoffDatetime = ConvertToUtc(record.TpepDropoffDatetime);
        record.StoreAndFwdFlag = record.StoreAndFwdFlag?.Trim().ToUpper() == "Y" ? "Yes" : "No";
    }

    private DateTime ConvertToUtc(DateTime dt)
    {
        DateTime unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecified, _estZone);
    }
}