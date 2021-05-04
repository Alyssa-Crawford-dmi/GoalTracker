﻿using GoalTracker.Models;
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
    public partial class AddGoal : ContentPage
    {
        public AddGoal()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(name.Text) && !String.IsNullOrEmpty(units.Text) && !String.IsNullOrEmpty(goal.Text))
            {
                Category newCat = new Category { Name = name.Text, TargetQuantity = int.Parse(goal.Text), Units = units.Text };
                await App.Database.SaveCategoryAsync(newCat);
                await Navigation.PopModalAsync();
            }
        }
    }
}