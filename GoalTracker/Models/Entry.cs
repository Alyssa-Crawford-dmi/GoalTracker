using System;
using System.Collections.Generic;
using System.Text;

namespace GoalTracker.Models
{
    public class Entry
    {
        public bool IsGoal { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }
}
