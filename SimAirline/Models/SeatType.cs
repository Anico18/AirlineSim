using System;
using System.Collections.Generic;

namespace SimAirline.Models
{
    public partial class SeatType
    {
        public SeatType()
        {
            BoardingPasses = new HashSet<BoardingPass>();
        }

        public int SeatTypeId { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<BoardingPass> BoardingPasses { get; set; }
    }
}
