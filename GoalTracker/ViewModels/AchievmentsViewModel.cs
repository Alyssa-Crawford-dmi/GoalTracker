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
        private IList<DisplayEntry> currentEntries;
        private DateTime dateViewing;

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
            Title = "Achievements";
            DateViewing = DateTime.Today;
        }

        public async void LoadData()
        {
            CurrentEntries = await App.Database.GetDisplyEntriesForDateAsync(DateViewing);
        }

        async void DeleteEntry(Object param)
        {
            int postId = (param as DisplayEntry).Id;
            await App.Database.DeleteEntryAsync(postId);
            LoadData();
        }

        async void AddEntry(Object param)
        {
            DisplayEntry curEntry = param as DisplayEntry;

            BasicEntry newEntry = new BasicEntry
            { CategoryName = curEntry.CategoryName, Date = dateViewing, IsGoal = false, Quantity = curEntry.Goal };
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
