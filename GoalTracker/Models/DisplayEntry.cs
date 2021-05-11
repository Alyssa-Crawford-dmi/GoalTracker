using System;

namespace GoalTracker.Models
{
    public class DisplayEntry
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public float Quantity { get; set; }
        public float Goal { get; set; }
        public string Units { get; set; }

    }
}
