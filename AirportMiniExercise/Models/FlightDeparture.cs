using Microsoft.EntityFrameworkCore;

namespace AirportWebAPI.Models
{
        [PrimaryKey("FlightNumber")]
        public class FlightDeparture
        {
                public int FlightNumber { get; set; }
                public string? Distination { get; set; }
                public TimeOnly DepartureTime { get; set; }
                public string? Gate { get; set; }
                public Status State { get; set; }


        }
        public enum Status
        {
                On_Time,
                Delayed,
                Boarding,
                Departed,
                Cancelled
        }

}
