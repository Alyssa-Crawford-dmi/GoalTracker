using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoalTracker.Models
{
    public class Category
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Units { get; set; }
        public float TargetQuantity { get; set; }

    }
}
