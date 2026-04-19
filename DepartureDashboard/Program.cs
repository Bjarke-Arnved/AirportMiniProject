using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using DepartureDashboard.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DepartureDashboard
{
        internal class Program
        {
                static async Task Main(string[] args)
                {
                        List<FlightDeparture> departures = new List<FlightDeparture>();


                        var factory = new ConnectionFactory() { HostName = "localhost" };
                        var connection = await factory.CreateConnectionAsync();
                        var channel = await connection.CreateChannelAsync();

                        await channel.ExchangeDeclareAsync(exchange: "departures", type: ExchangeType.Fanout);
                        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync();
                        string queueName = queueDeclareResult.QueueName;
                        await channel.QueueBindAsync(queue: queueName, exchange: "departures", routingKey: string.Empty);

                        // Awaiting departures
                        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
                        await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
                        consumer.ReceivedAsync += (model, ea) =>
                        {
                                byte[] body = ea.Body.ToArray();
                                var message = Encoding.UTF8.GetString(body);
                                FlightDeparture? departure = JsonSerializer.Deserialize<FlightDeparture>(message);
                                if (departure != null)
                                {
                                        departures.Add(departure);
                                        Console.Clear();
                                        DisplayDepartures(departures);
                                }
                                return Task.CompletedTask;
                        };
                        Console.ReadLine();
                }
                public static void DisplayDepartures(List<FlightDeparture> departures)
                {
                        Console.WriteLine("Departures:");
                        Console.WriteLine("Flight Number | Distination | Time | Gate | Status");
                        foreach(FlightDeparture departure in departures)
                        {
                                Console.WriteLine(departure.FlightNumber + " " + departure.Distination + " " + departure.DepartureTime + " " + departure.Gate + " " + departure.State);
                        }
                }
        }
}
