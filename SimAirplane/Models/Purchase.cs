using System;
using System.Collections.Generic;

namespace SimAirplane.Models
{
    public partial class Purchase
    {
        public Purchase()
        {
            BoardingPasses = new HashSet<BoardingPass>();
        }

        public int PurchaseId { get; set; }
        public int PurchaseDate { get; set; }

        public virtual ICollection<BoardingPass> BoardingPasses { get; set; }
    }
}
