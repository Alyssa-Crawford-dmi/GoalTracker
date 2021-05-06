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
        string categoryName = "Squats";
        DateTime startDate = DateTime.Today;
        DateTime endDate = DateTime.Today.AddDays(7);

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

            List<BasicEntry> goalEntries = await App.Database.getTrendEntries(categoryName, true, startDate, endDate);
            List<BasicEntry> achievementEntries = await App.Database.getTrendEntries(categoryName, false, startDate, endDate);

            goals = await ConvertToChartEntriesRepeatPrevIfNoEntry(goalEntries, "#AAA", startDate, endDate);
            achievements = ConvertToChartEntriesZeroNoEntry(achievementEntries, "#2F8789", startDate, endDate);

            goalsChart.Chart = SharedChart(goals, true);
            achievementsChart.Chart = SharedChart(achievements, false);
        }

        private Chart SharedChart(List<ChartEntry> list, bool showLabels)
        {
            return new LineChart
            {
                Entries = list,
                LineMode = LineMode.Straight,
                BackgroundColor = SKColor.Empty,
                LabelOrientation = Orientation.Horizontal,
                LabelColor = showLabels ? SKColor.Parse("#AAA") : SKColor.Empty,
                LineAreaAlpha = showLabels ? (byte)32 : (byte)0,
                LabelTextSize = 20,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint
                {
                    TextSize = 40,
                },
                MinValue = 15,
                MaxValue = 25

            };
        }

        private List<ChartEntry> ConvertToChartEntriesZeroNoEntry(List<BasicEntry> entries, string color, DateTime startDate, DateTime endDate)
        {
            List<ChartEntry> results = new List<ChartEntry>();
            DateTime curDate = startDate;
            int entryIndex = 0;
            while (entryIndex < entries.Count && entries[entryIndex].Date < curDate)
            {
                entryIndex++;
            }
            while (curDate <= endDate)
            {
                if (entryIndex >= entries.Count || entries[entryIndex].Date != curDate)
                {
                    results.Add(new ChartEntry(0) { Label = curDate.ToShortDateString(), Color = SKColor.Parse(color) });
                }
                else
                {
                    BasicEntry entry = entries[entryIndex];
                    results.Add(new ChartEntry(entry.Quantity) { Label = curDate.ToShortDateString(), Color = SKColor.Parse(color) });
                    entryIndex += 1;
                }
                curDate = curDate.AddDays(1);
            }
            return results;
        }

        private async Task<List<ChartEntry>> ConvertToChartEntriesRepeatPrevIfNoEntry(List<BasicEntry> entries, string color, DateTime startDate, DateTime endDate)
        {
            List<ChartEntry> results = new List<ChartEntry>();
            DateTime curDate = startDate;
            int entryIndex = 0;
            BasicEntry lastEntry = null;
            while (entryIndex < entries.Count && entries[entryIndex].Date < curDate)
            {
                entryIndex++;
            }
            while (curDate <= endDate)
            {
                if (entryIndex >= entries.Count || entries[entryIndex].Date != curDate)
                {
                    if (lastEntry == null)
                    {
                        lastEntry = await App.Database.FetchPriorGoalEntry(categoryName, curDate);
                        if (lastEntry == null)
                        {
                            lastEntry = new BasicEntry { Quantity = 0 };
                        }
                    }
                    results.Add(new ChartEntry(lastEntry.Quantity) { Label = curDate.ToShortDateString(), Color = SKColor.Parse(color) });
                }
                else
                {
                    lastEntry = entries[entryIndex];
                    results.Add(new ChartEntry(lastEntry.Quantity) { Label = curDate.ToShortDateString(), Color = SKColor.Parse(color) });
                    entryIndex += 1;
                }
                curDate = curDate.AddDays(1);
            }
            return results;
        }
    }
}