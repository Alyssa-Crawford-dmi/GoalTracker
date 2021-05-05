using GoalTracker.Models;
using Microcharts;
using SkiaSharp;
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
    public partial class Trends : ContentPage
    {
        public Trends()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            LoadChart();
            base.OnAppearing();
        }

        private async void LoadChart()
        {
            List<ChartEntry> goals;
            List<ChartEntry> achievements;

            List<BasicEntry> goalEntries = await App.Database.getTrendEntries("Squats", true);
            List<BasicEntry> achievementEntries = await App.Database.getTrendEntries("Squats", false);

            //goals = ConvertToChartEntries(goalEntries, "#2c3e50");
            achievements = ConvertToChartEntriesZeroNoEntry(achievementEntries, "#3498db", DateTime.Today, DateTime.Today.AddDays(7));

            //goalsChart.Chart = SharedChart(goals);
            achievementsChart.Chart = SharedChart(achievements);
        }

        private Chart SharedChart(List<ChartEntry> list)
        {
            return new LineChart
            {
                Entries = list,
                LineMode = LineMode.Straight,
                BackgroundColor = SKColor.Empty,
                LabelOrientation = Orientation.Horizontal,
                LabelTextSize = 20,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint
                {
                    TextSize = 40,
                },
                MinValue = 0,
                MaxValue = 25

            };
        }

        private List<ChartEntry> ConvertToChartEntriesZeroNoEntry(List<BasicEntry> entries, string color, DateTime startDate, DateTime endDate)
        {
            List<ChartEntry> results = new List<ChartEntry>();
            DateTime curDate = startDate;
            int entryIndex = 0;
            while (curDate <= endDate)
            {
                if (entryIndex >= entries.Count || entries[entryIndex].Date != curDate)
                {
                    results.Add(new ChartEntry(0) { Label = curDate.ToShortDateString(), Color = SKColor.Parse(color) });
                }
                else
                {
                    BasicEntry entry = entries[entryIndex];
                    results.Add(new ChartEntry(entry.Quantity) { Label = entry.Date.ToShortDateString(), Color = SKColor.Parse(color) });
                    entryIndex += 1;
                }
                curDate = curDate.AddDays(1);
            }
            return results;
        }
    }
}