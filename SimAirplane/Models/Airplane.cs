using System;
using System.Collections.Generic;

namespace SimAirplane.Models
{
    public partial class Airplane
    {
        public Airplane()
        {
            Seats = new HashSet<Seat>();
        }

        public int AirplaneId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Seat> Seats { get; set; }
    }
}
