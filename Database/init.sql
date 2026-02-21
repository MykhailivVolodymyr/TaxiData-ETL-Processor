CREATE DATABASE TaxiDb;
GO

USE TaxiDb;
GO

CREATE TABLE TaxiTrips (
    tpep_pickup_datetime DATETIME2 NOT NULL,
    tpep_dropoff_datetime DATETIME2 NOT NULL,
    passenger_count INT NULL,
    trip_distance FLOAT NOT NULL,
    store_and_fwd_flag NVARCHAR(3) NOT NULL,
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    fare_amount DECIMAL(10, 2) NOT NULL,
    tip_amount DECIMAL(10, 2) NOT NULL
);
GO

CREATE INDEX IX_TaxiTrips_PULocationID ON TaxiTrips (PULocationID) INCLUDE (tip_amount);

CREATE INDEX IX_TaxiTrips_TripDistance ON TaxiTrips (trip_distance DESC);

CREATE INDEX IX_TaxiTrips_TravelDuration ON TaxiTrips (tpep_pickup_datetime, tpep_dropoff_datetime);