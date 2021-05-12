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
        public string categoryName;
        decimal minVal = 0;
        decimal maxVal = 0;

        public Trends()
        {
            InitializeComponent();
            startDate.Date = DateTime.Today.AddDays(-7);
            DisplayKey();
        }

        public Trends(DateTime newStartDate, DateTime newEndDate) : this()
        {
            startDate.Date = newStartDate;
            endDate.Date = newEndDate;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadChart();
            contentPage.Title = $"Trends - {categoryName}";
        }
        private void DisplayKey()
        {
            var goalEntries = new[] {
                new ChartEntry(0) { Color = SKColor.Empty, Label = "Goal" },
                new ChartEntry(5) { Color = SKColor.Empty}};
            GoalsKey.Chart = new LineChart
            {
                Entries = goalEntries,
                LineMode = LineMode.Straight,
                BackgroundColor = SKColor.Empty,
                LabelOrientation = Orientation.Horizontal,
                LabelTextSize = 40
            };
            var achievmentEntries = new[] {
                new ChartEntry(0) { Color = SKColor.Parse("#2F8789"), Label = "Entries" },
                new ChartEntry(5) { Color = SKColor.Parse("#2F8789") } };
            AchievementsKey.Chart = new LineChart
            {
                Entries = achievmentEntries,
                LineMode = LineMode.Straight,
                BackgroundColor = SKColor.Empty,
                LabelOrientation = Orientation.Horizontal,
                LineAreaAlpha = (byte)0,
                LabelTextSize = 40,
            };
        }

        private async void LoadChart()
        {
            minVal = 0;
            maxVal = 0;
            updateAlertAndKeyVisablity();
            if (categoryName == null || startDate.Date > endDate.Date)
            {
                goalsChart.Chart = null;
                achievementsChart.Chart = null;
                return;
            }
            List<ChartEntry> goals;
            List<ChartEntry> achievements;
            List<ChartEntry> labels;

            List<BasicEntry> goalEntries = await App.Database.getTrendEntries(categoryName, true, startDate.Date, endDate.Date);
            List<BasicEntry> achievementEntries = await App.Database.getTrendEntries(categoryName, false, startDate.Date, endDate.Date);

            goals = await ConvertToChartEntriesRepeatPrevIfNoEntry(goalEntries, SKColor.Empty, startDate.Date, endDate.Date);
            achievements = ConvertToChartEntriesZeroNoEntry(achievementEntries, SKColor.Parse("#2F8789"), startDate.Date, endDate.Date);
            labels = populateLabels();
            //bool useAchievmentLabels = achievementEntries.Count > 0;

            goalsChart.Chart = SharedChart(goals, false, true);
            achievementsChart.Chart = SharedChart(achievements, false);
            labelsChart.Chart = SharedChart(labels, true);
        }

        private void updateAlertAndKeyVisablity()
        {
            keyGroup.IsVisible = false;
            categoryAlertText.IsVisible = false;
            dateAlertText.IsVisible = false;
            if (categoryName == null)
            {
                categoryAlertText.IsVisible = true;
            }
            else if (startDate.Date > endDate.Date)
            {
                dateAlertText.IsVisible = true;
            }
            else
            {
                keyGroup.IsVisible = true;
            }
        }

        private List<ChartEntry> populateLabels()
        {
            List<ChartEntry> entries = new List<ChartEntry>();
            DateTime curDate = startDate.Date;
            entries.Add(new ChartEntry((float)minVal) { Label = getLabel(curDate), Color = SKColor.Empty });
            curDate = curDate.AddDays(1);
            int labelFrequeny = calculateLabelFrequency();
            while (curDate < endDate.Date)
            {
                int daysDiff = (curDate - startDate.Date).Days;
                if (daysDiff % labelFrequeny == 0)
                {
                    entries.Add(new ChartEntry((float)maxVal) { Label = getLabel(curDate), Color = SKColor.Empty });
                }
                curDate = curDate.AddDays(1);
            }
            entries.Add(new ChartEntry((float)maxVal) { Label = getLabel(curDate), Color = SKColor.Empty });
            return entries;
        }

        private int calculateLabelFrequency()
        {
            int daysDiff = (endDate.Date - startDate.Date).Days;
            if (daysDiff < 14)
            {
                return 1;
            }
            //if (daysDiff < 7)
            //{
            //    return 2;
            //}
            if (daysDiff < 30)
            {
                return 5;
            }
            //if (daysDiff < 90)
            //{
            //    return 15;
            //}
            return 30;
        }

        private Chart SharedChart(List<ChartEntry> list, bool showLabels, bool showShadow = false)
        {
            return new LineChart
            {
                Entries = list,
                LineMode = showLabels ? LineMode.None : LineMode.Straight,
                BackgroundColor = SKColor.Empty,
                LabelOrientation = Orientation.Horizontal,
                LabelColor = showLabels ? SKColor.Parse("#AAA") : SKColor.Empty,
                LineAreaAlpha = showShadow ? (byte)32 : (byte)0,
                LabelTextSize = 20,
                ShowYAxisLines = showLabels,
                ShowYAxisText = true,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint
                {
                    TextSize = 40,
                    Color = showLabels ? SKColor.Parse("#AAA") : SKColor.Empty
                },
                MinValue = (float)minVal,
                MaxValue = (float)maxVal

            };
        }

        private List<ChartEntry> ConvertToChartEntriesZeroNoEntry(List<BasicEntry> entries, SKColor color, DateTime startDate, DateTime endDate)
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
                    results.Add(new ChartEntry(0) { Label = getLabel(curDate), Color = color });
                }
                else
                {
                    BasicEntry entry = entries[entryIndex];
                    results.Add(new ChartEntry((float)entry.Quantity) { Label = getLabel(curDate), Color = color });
                    entryIndex += 1;
                    checkAndUpdateMinMax(entry);
                }
                curDate = curDate.AddDays(1);
            }
            return results;
        }
        private async Task<List<ChartEntry>> ConvertToChartEntriesRepeatPrevIfNoEntry(List<BasicEntry> entries, SKColor color, DateTime startDate, DateTime endDate)
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
                    results.Add(new ChartEntry((float)lastEntry.Quantity) { Label = getLabel(curDate), Color = color });
                }
                else
                {
                    lastEntry = entries[entryIndex];
                    results.Add(new ChartEntry((float)lastEntry.Quantity) { Label = getLabel(curDate), Color = color });
                    entryIndex += 1;
                }
                checkAndUpdateMinMax(lastEntry);
                curDate = curDate.AddDays(1);
            }
            return results;
        }

        private string getLabel(DateTime curDate)
        {
            return curDate.ToString("M/d");
        }

        private void checkAndUpdateMinMax(BasicEntry entry)
        {
            if (entry.Quantity < minVal) minVal = entry.Quantity;
            if (entry.Quantity > maxVal) maxVal = entry.Quantity;
        }

        //private void Categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Category category = e.CurrentSelection[0] as Category;
        //    categoryName = category.Name;
        //    LoadChart();
        //}

        private void dateSelected(object sender, DateChangedEventArgs e)
        {
            LoadChart();
        }
    }
}