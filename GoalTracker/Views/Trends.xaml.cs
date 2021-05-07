using GoalTracker.Models;
using GoalTracker.ViewModels;
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
        string categoryName;
        int minVal = 0;
        int maxVal = 0;

        public Trends()
        {
            InitializeComponent();
            endDate.Date = DateTime.Today.AddDays(7);
        }

        protected override void OnAppearing()
        {
            (BindingContext as TrendsViewModel).LoadData();
            base.OnAppearing();
        }

        private async void LoadChart()
        {
            List<ChartEntry> goals;
            List<ChartEntry> achievements;

            List<BasicEntry> goalEntries = await App.Database.getTrendEntries(categoryName, true, startDate.Date, endDate.Date);
            List<BasicEntry> achievementEntries = await App.Database.getTrendEntries(categoryName, false, startDate.Date, endDate.Date);

            goals = await ConvertToChartEntriesRepeatPrevIfNoEntry(goalEntries, "#AAA", startDate.Date, endDate.Date);
            achievements = ConvertToChartEntriesZeroNoEntry(achievementEntries, "#2F8789", startDate.Date, endDate.Date);

            goalsChart.Chart = SharedChart(goals, false);
            achievementsChart.Chart = SharedChart(achievements, true);
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
                LineAreaAlpha = showLabels ? (byte)0 : (byte)32,
                LabelTextSize = 30,
                ShowYAxisLines = showLabels,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint
                {
                    TextSize = 40,
                    Color = showLabels ? SKColor.Parse("#AAA") : SKColor.Empty
                },
                MinValue = minVal,
                MaxValue = maxVal

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
                    results.Add(new ChartEntry(0) { Label = getLabel(curDate), Color = SKColor.Parse(color) });
                }
                else
                {
                    BasicEntry entry = entries[entryIndex];
                    results.Add(new ChartEntry(entry.Quantity) { Label = getLabel(curDate), Color = SKColor.Parse(color) });
                    entryIndex += 1;
                    checkAndUpdateMinMax(entry);
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
                    results.Add(new ChartEntry(lastEntry.Quantity) { Label = getLabel(curDate), Color = SKColor.Parse(color) });
                }
                else
                {
                    lastEntry = entries[entryIndex];
                    results.Add(new ChartEntry(lastEntry.Quantity) { Label = getLabel(curDate), Color = SKColor.Parse(color) });
                    entryIndex += 1;
                }
                checkAndUpdateMinMax(lastEntry);
                curDate = curDate.AddDays(1);
            }
            return results;
        }

        private string getLabel(DateTime curDate)
        {
            return curDate.ToString("MM/dd");
        }

        private void checkAndUpdateMinMax(BasicEntry entry)
        {
            if (entry.Quantity < minVal) minVal = entry.Quantity;
            if (entry.Quantity > maxVal) maxVal = entry.Quantity;
        }

        private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category category = e.CurrentSelection[0] as Category;
            categoryName = category.Name;
            LoadChart();
        }

        private void dateSelected(object sender, DateChangedEventArgs e)
        {
            LoadChart();
        }
    }
}