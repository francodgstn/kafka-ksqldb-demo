# Kafka ksqlDB demo

Pretty much similar example as in the [ksqlDB quick start](https://ksqldb.io/quickstart.html), just with airline related examples.

## Setting up the environment

```bash
./demo.ps1
```

Or manually:

- Run the doker images
  
  ```bash
  docker-compose up -d
  ```

- Create a topic `flights`

  ```bash
  docker exec -it broker kafka-topics --create --topic flights --bootstrap-server broker:9092 --partitions 1 --replication-factor 1
  ```

- Run the dummy flights producer app (.net)

  ```bash
  cd KafkaFLightProducer
  dotnet run
  ```

Then open a ksqlDB CLI terminal, and tryout the examples below:

```bash
docker exec -it ksqldb-cli ksql http://ksqldb-server:8088
```

## Create a stream

A stream in ksqlDB represents an unbounded sequence of structured data (events) in Kafka.

```sql
CREATE STREAM flights_stream (
  flight_number INT,
  airline_code STRING,
  departure STRING,
  arrival STRING,
  status STRING
) WITH (
  KAFKA_TOPIC='flights',
  VALUE_FORMAT='JSON'
);
```

```sql
select * from flights_stream EMIT CHANGES;
```

## Example 1 - Aggregation

Table (materialized view) - A table in ksqlDB represents a materialized view of the latest state of data, derived from a stream.

```sql
CREATE TABLE airlineCount AS
    SELECT airline_code, COUNT(*) AS flight_count
    FROM flights_stream
    GROUP BY airline_code
    EMIT CHANGES;
```

```sql
select * from airlineCount EMIT CHANGES;
```

## Example 2 - Filtering

Create a stream for cancelled flights.

```sql
CREATE STREAM cancelled_flights_stream AS
    SELECT *
    FROM flights_stream
    WHERE status = 'CANCELLED'
    EMIT CHANGES;
```

Run the "push" query

```sql
select * from cancelled_flights_stream EMIT CHANGES;
```
