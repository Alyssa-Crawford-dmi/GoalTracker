using System;

namespace GoalTracker.Models
{
    public class DisplayEntry
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Goal { get; set; }
        public string Units { get; set; }

    }
}
