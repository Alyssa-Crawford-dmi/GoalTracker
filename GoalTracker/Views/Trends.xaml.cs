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
        private readonly ChartEntry[] entries = new[]
        {
            new ChartEntry(212)
            {
                Label = "UWP",
                ValueLabel = "112",
                Color = SKColor.Parse("#2c3e50")
            },
            new ChartEntry(248)
            {
                Label = "Android",
                ValueLabel = "648",
                Color = SKColor.Parse("#77d065")
            },
            new ChartEntry(128)
            {
                Label = "iOS",
                ValueLabel = "428",
                Color = SKColor.Parse("#b455b6")
            },
            new ChartEntry(514)
            {
                Label = "Forms",
                ValueLabel = "214",
                Color = SKColor.Parse("#3498db")
            }
        };

        public Trends()
        {
            InitializeComponent();
            List<ChartEntry> goals = new List<ChartEntry>();
            List<ChartEntry> achievements = new List<ChartEntry>();
            for (int i = 0; i < 20; i++)
            {
                goals.Add(new ChartEntry(new Random().Next(1, 11)) { Color = SKColor.Parse("A4DFE0") });
                achievements.Add(new ChartEntry(new Random().Next(1, 11)) { Color = SKColor.Parse("2F8789") });
            }

            chartView1.Chart = new BarChart { Entries = goals, BackgroundColor = SKColor.Empty };
            chartView2.Chart = new LineChart { Entries = achievements, BackgroundColor = SKColor.Empty, LineMode = LineMode.Straight };
        }
    }
}