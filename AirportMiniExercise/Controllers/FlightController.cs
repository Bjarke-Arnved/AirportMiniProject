using System.Text.Json;
using System.Threading.Channels;
using AirportWebAPI.Db;
using AirportWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace AirportWebAPI.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class FlightController : Controller
        {
                private readonly IChannel _channel;
                private readonly FlightContext _flightContext;

                public FlightController(IChannel channel, FlightContext flightContext)
                {
                        _channel = channel;
                        _flightContext = flightContext;
                }
                        
                public IActionResult Index()
                {
                        return View();
                }

                [HttpPost("api/departures")]
                public async Task<ActionResult<FlightDeparture>> PostFlight(FlightDeparture flight)
                {
                        _flightContext.FlightDepartures.Add(flight);
                        await _flightContext.SaveChangesAsync();
                        var message = JsonSerializer.Serialize(flight);
                        var body = System.Text.Encoding.UTF8.GetBytes(message);
                        await _channel.BasicPublishAsync(exchange: "departures", routingKey: string.Empty, body: body);
                        return Ok();
                }

                [HttpGet("{flightNumber}")]
                public async Task<ActionResult<FlightDeparture>> GetDeparture(int flightNumber)
                {
                        var flightDeparture = await _flightContext.FlightDepartures.FindAsync(flightNumber);
                        if(flightDeparture == null)
                        {
                                return NotFound();
                        }
                        return flightDeparture;
                }
        }
}
