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
    public partial class Goals : ContentPage
    {
        public Goals()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as GoalsViewModel).LoadData();
            base.OnAppearing();
        }

        private async void Add_goal(object sender, EventArgs e)
        {
            var addGoal = new AddGoal();
            await Navigation.PushAsync(addGoal);

        }
    }
}