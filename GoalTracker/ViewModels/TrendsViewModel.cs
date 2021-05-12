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

        private DateTime startDate;
        private DateTime endDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }


        public TrendsViewModel()
        {
            Title = "Trends";
            SelectionCommand = new Command(SelectionMade);
            LoadData();
        }

        private async void SelectionMade(object obj)
        {
            Category category = obj as Category;
            var trends = new Trends(StartDate, EndDate)
            {
                categoryName = category.Name
            };
            await Application.Current.MainPage.Navigation.PushAsync(trends);
        }

        public async void LoadData()
        {
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

    }
}
