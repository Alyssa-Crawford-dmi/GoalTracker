﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GoalTracker.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Units { get; set; }
        public int CurrentGoal { get; set; }
        public string Name { get; set; } 
    }
}