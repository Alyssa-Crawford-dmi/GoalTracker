using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class GoalsViewModel : BaseViewModel
    {
        private IList<Category> currentGoals;

        public IList<Category> CurrentGoals { get => currentGoals; set => SetProperty(ref currentGoals, value); }

        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }

        public GoalsViewModel()
        {
            Title = "Goals";
            DeleteCommand = new Command(DeleteGoalAsync);
            UpdateCommand = new Command(UpdateGoal);
            LoadData();
        }

        private async void DeleteGoalAsync(object obj)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Are you sure you want to delete this goal?", "All Corresponding entries will also be deleted", "Delete", "Cancel");
            if (answer)
            {
                int goalId = (obj as Category).Id;
                await App.Database.DeleteCategoryAsync(goalId);
                CurrentGoals = await App.Database.GetCategoriesAsync();
            }
        }

        private async void UpdateGoal(object obj)
        {
            Category goal = obj as Category;
            await App.Database.UpdateCategoryAsync(goal);
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

        private async void LoadData()
        {
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }
    }
}
