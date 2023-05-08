using System;
using System.Collections.Generic;

namespace SimAirline.Models
{
    public partial class Flight
    {
        public Flight()
        {
            BoardingPasses = new HashSet<BoardingPass>();
        }

        public int FlightId { get; set; }
        public int? TakeoffDateTime { get; set; }
        public string? TakeoffAirport { get; set; }
        public int? LandingDateTime { get; set; }
        public string? LandingAirport { get; set; }
        public int? AirplaneId { get; set; }

        public virtual ICollection<BoardingPass> BoardingPasses { get; set; }
        public virtual Airplane Airplanes { get; set; }
    }
}
