using GoalTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoalTracker.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Achievments : ContentPage
    {
        public Achievments()
        {
            InitializeComponent();
        }

        private async void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            int enteredVal = int.Parse((sender as Entry).Text);
            BasicEntry goalEntry = new BasicEntry { IsGoal = false, CategoryId = 1, Date = DateTime.Now, Quantity = enteredVal };
            await App.Database.SaveEntryAsync(goalEntry);
            Entries.ItemsSource = await App.Database.GetDisplyEntriesForDateAsync();
        }

    }
}