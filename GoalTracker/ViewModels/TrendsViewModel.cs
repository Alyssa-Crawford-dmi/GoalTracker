using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using GoalTracker.Views;

namespace GoalTracker.ViewModels
{
    public class TrendsViewModel : BaseViewModel
    {
        private IList<Category> currentGoals;
        public IList<Category> CurrentGoals { get => currentGoals; set => SetProperty(ref currentGoals, value); }
        public ICommand SelectionCommand { get; }

        public TrendsViewModel()
        {
            Title = "Trends";
            SelectionCommand = new Command(SelectionMade);
            LoadData();
        }

        private async void SelectionMade(object obj)
        {
            Category category = obj as Category;
            var trends = new Trends
            {
                categoryName = category.Name
            };
            await Application.Current.MainPage.Navigation.PushAsync(trends);
        }

        public async void LoadData()
        {
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

    }
}
