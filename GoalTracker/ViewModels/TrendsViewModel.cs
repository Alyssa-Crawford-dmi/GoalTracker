using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace GoalTracker.ViewModels
{
    public class TrendsViewModel : BaseViewModel
    {
        private IList<Category> currentGoals;
        public IList<Category> CurrentGoals { get => currentGoals; set => SetProperty(ref currentGoals, value); }

        public TrendsViewModel()
        {
            Title = "Trends";
            LoadData();
        }

        public async void LoadData()
        {
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

    }
}
