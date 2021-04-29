using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoalTracker.ViewModels
{
    public class AchievmentsViewModel:BaseViewModel
    {
        public IList<Category> CurrentGoals { get; private set; }

        public AchievmentsViewModel()
        {
            Title = "Achievments";
            CurrentGoals = new List<Category>();
            CurrentGoals.Add(new Category{Id=1, Units="Reps", CurrentGoal=20, Name="Squats" });
            CurrentGoals.Add(new Category{Id=2, Units="Reps", CurrentGoal=10, Name="Push-ups" });
            CurrentGoals.Add(new Category{Id=3, Units="Reps", CurrentGoal=40, Name="Sit-ups" });
            CurrentGoals.Add(new Category{Id=4, Units="Min", CurrentGoal=15, Name="Walking" });
        }
    }
}
