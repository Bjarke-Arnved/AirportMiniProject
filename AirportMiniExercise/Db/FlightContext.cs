using AirportWebAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace AirportWebAPI.Db
{
        public class FlightContext : DbContext
        {
                public FlightContext(DbContextOptions<FlightContext> options) : base(options) { }

                public DbSet<FlightDeparture> FlightDepartures { get; set; } = null!;
        }
}
