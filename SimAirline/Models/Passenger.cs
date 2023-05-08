using System;
using System.Collections.Generic;

namespace SimAirline.Models
{
    public partial class Passenger
    {
        public Passenger()
        {
            BoardingPasses = new HashSet<BoardingPass>();
        }

        public int PassengerId { get; set; }
        public string? Dni { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Country { get; set; }

        public virtual ICollection<BoardingPass> BoardingPasses { get; set; }
    }
}
