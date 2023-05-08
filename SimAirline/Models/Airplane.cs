using System;
using System.Collections.Generic;

namespace SimAirline.Models
{
    public partial class Airplane
    {
        public Airplane()
        {
            Seats = new HashSet<Seat>();
            Flights = new HashSet<Flight>();
        }

        public int AirplaneId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Seat> Seats { get; set; }
        public virtual ICollection<Flight> Flights { get; set; }
    }
}
