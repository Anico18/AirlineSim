using System;
using System.Collections.Generic;

namespace SimAirplane.Models
{
    public partial class Seat
    {
        public Seat()
        {
            BoardingPasses = new HashSet<BoardingPass>();
        }

        public int SeatId { get; set; }
        public string? SeatColumn { get; set; }
        public int? SeatRow { get; set; }
        public int? SeatTypeId { get; set; }
        public int? AirplaneId { get; set; }

        public virtual Airplane? Airplane { get; set; }
        public virtual ICollection<BoardingPass> BoardingPasses { get; set; }
    }
}
