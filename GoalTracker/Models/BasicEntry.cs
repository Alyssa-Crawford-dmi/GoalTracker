using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoalTracker.Models
{
    public class BasicEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool IsGoal { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public float Quantity { get; set; }


    }
}
