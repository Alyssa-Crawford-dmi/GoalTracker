using System;

namespace GoalTracker.Models
{
    public class DisplayEntry
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string Units { get; set; }

    }
}
