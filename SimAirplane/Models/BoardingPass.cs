using System;
using System.Collections.Generic;

namespace SimAirplane.Models
{
    public partial class BoardingPass
    {
        public int BoardingPassId { get; set; }
        public int? PurchaseId { get; set; }
        public int? PassengerId { get; set; }
        public int? SeatTypeId { get; set; }
        public int? SeatId { get; set; }
        public int? FlightId { get; set; }

        public virtual Flight? Flight { get; set; }
        public virtual Passenger? Passenger { get; set; }
        public virtual Purchase? Purchase { get; set; }
        public virtual Seat? Seat { get; set; }
        public virtual SeatType? SeatType { get; set; }
    }
}
