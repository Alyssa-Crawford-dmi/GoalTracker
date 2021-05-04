using GoalTracker.Models;
using GoalTracker.ViewModels;
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
        protected override void OnAppearing()
        {
            (BindingContext as AchievmentsViewModel).LoadData();
            base.OnAppearing();
        }
    }
}