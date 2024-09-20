using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace KafkaFlightProducer
{
    class Program
    {
        private static readonly string[] AirlineCodes = { "LX", "LH", "WK", "SN" };
        private static readonly string[] Airports = { "ZRH", "BSL", "GVA", "JFK", "ATL", "LAX", "ORD", "SFO", "DEN" };
        private static readonly string[] Statuses = { "ON_TIME", "DELAYED", "CANCELLED" };

        static async Task Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:29092" };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            var random = new Random();

            while (true)
            {
                var flightData = new
                {
                    airline_code = AirlineCodes[random.Next(AirlineCodes.Length)],
                    flight_number = random.Next(1, 1000),
                    departure = Airports[random.Next(Airports.Length)],
                    arrival = Airports[random.Next(Airports.Length)],
                    status = Statuses[random.Next(Statuses.Length)],
                };

                var flightDataJson = JsonSerializer.Serialize(flightData);

                await producer.ProduceAsync("flights", new Message<Null, string> { Value = flightDataJson });

                Console.WriteLine($"Produced: {flightDataJson}");

                await Task.Delay(1000); // Sleep for 1 second before sending the next message
            }
        }

        private static DateTime RandomDate(Random random, DateTime startDate, DateTime endDate)
        {
            int range = (endDate - startDate).Days;
            return startDate.AddDays(random.Next(range));
        }
    }
}