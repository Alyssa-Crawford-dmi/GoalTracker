using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GoalTracker.ViewModels
{
    public class AchievmentsViewModel:BaseViewModel
    {
        private IList<Category> currentGoals;
        public IList<DisplayEntry> currentEntries;
        public IList<Category> CurrentGoals 
        { 
            get=> currentGoals; 
            private set => SetProperty(ref currentGoals, value); 
        }
        public IList<DisplayEntry> CurrentEntries 
        { 
            get => currentEntries;
            private set => SetProperty(ref currentEntries, value); 
        }

        public ICommand DeleteCommand { get; }

        public AchievmentsViewModel()
        {
            DeleteCommand = new Command(DeleteEntry);
            Title = "Achievments";
            LoadData();
        }

        private async void LoadData()
        {
            CurrentEntries = await App.Database.GetDisplyEntriesForDateAsync();
            CurrentGoals = await App.Database.GetCategoriesAsync();
        }

        async void DeleteEntry(Object param)
        {
            int postId = (param as DisplayEntry).Id;
            await App.Database.DeleteEntryAsync(postId);
            CurrentEntries =await  App.Database.GetDisplyEntriesForDateAsync();
        }

    }
}
