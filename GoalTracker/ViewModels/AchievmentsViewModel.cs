using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class AchievmentsViewModel : BaseViewModel
    {
        private IList<Category> currentGoals;
        private IList<DisplayEntry> currentEntries;
        private DateTime dateViewing;

        public IList<Category> CurrentGoals
        {
            get => currentGoals;
            private set => SetProperty(ref currentGoals, value);
        }
        public IList<DisplayEntry> CurrentEntries
        {
            get => currentEntries;
            private set => SetProperty(ref currentEntries, value);
        }
        public DateTime DateViewing
        {
            get => dateViewing;
            set
            {
                SetProperty(ref dateViewing, value);
                LoadData();
                Console.WriteLine(DateViewing);
            }
        }

        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DecreaseDateCommand { get; }
        public ICommand IncreaseDateCommand { get; }

        public AchievmentsViewModel()
        {
            DeleteCommand = new Command(DeleteEntry);
            AddCommand = new Command(AddEntry);
            DecreaseDateCommand = new Command(DecreaseDate);
            IncreaseDateCommand = new Command(IncreaseDate);
            Title = "Achievments";
            DateViewing = DateTime.Today;
            LoadData();
        }

        private async void LoadData()
        {
            CurrentEntries = await App.Database.GetDisplyEntriesForDateAsync(DateViewing);
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

        async void DeleteEntry(Object param)
        {
            int postId = (param as DisplayEntry).Id;
            await App.Database.DeleteEntryAsync(postId);
            CurrentEntries = await App.Database.GetDisplyEntriesForDateAsync(DateViewing);
        }

        async void AddEntry(Object param)
        {
            Category goal = param as Category;
            BasicEntry newEntry = new BasicEntry
            { CategoryId = goal.Id, Date = dateViewing, IsGoal = false, Quantity = goal.TargetQuantity };
            await App.Database.SaveEntryAsync(newEntry);
            LoadData();
        }

        private void IncreaseDate(object obj)
        {
            DateViewing = DateViewing.AddDays(1);
        }

        private void DecreaseDate(object obj)
        {
            DateViewing = DateViewing.AddDays(-1);
        }

    }
}
