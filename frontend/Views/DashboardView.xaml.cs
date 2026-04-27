using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataAnalizer.Models;
using DataAnalizer.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace DataAnalizer.Views
{
    public partial class DashboardView : UserControl
    {
        private readonly ApiService _apiService;

        public DashboardView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ = LoadLatestDataAsync();
        }

        private async Task LoadLatestDataAsync()
        {
            try
            {
                var logs = await _apiService.GetLogsAsync();
                if (logs.Any())
                {
                    var latestLog = logs.First();
                    var stats = await _apiService.GetLogDetailsAsync(latestLog.Id);
                    UpdateDashboard(stats);
                }
            }
            catch (Exception ex)
            {
                // Handle error quietly or show a toast
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateDashboard(AnalyticsData stats)
        {
            if (stats == null) return;

            // Update KPIs
            TxtTotalOffers.Text = stats.TotalOffers.ToString("N0");
            TxtAvgPrice.Text = $"${stats.AvgPrice:N0}";
            TxtMedianPrice.Text = $"${stats.MedianPrice:N0}";
            TxtAvgPricePerSqm.Text = $"${stats.AvgPricePerSqm:N0}";

            // Update Bar Chart (Top Cities)
            if (stats.TopCities != null)
            {
                BarChartCities.Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = stats.TopCities.Select(c => c.PricePerSqm).ToArray(),
                        Fill = new SolidColorPaint(SKColor.Parse("#4f46e5")),
                        MaxBarWidth = 30,
                        TooltipLabelFormatter = (chartPoint) => $"${chartPoint.PrimaryValue:N0}/m²"
                    }
                };

                BarChartCities.XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = stats.TopCities.Select(c => c.Name).ToArray(),
                        LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748b"))
                    }
                };
            }

            // Update Pie Chart (Room Distribution)
            if (stats.RoomDistribution != null)
            {
                var colors = new[] { "#4f46e5", "#6366f1", "#818cf8", "#a5b4fc", "#c7d2fe" };
                var series = new List<ISeries>();
                for (int i = 0; i < stats.RoomDistribution.Count; i++)
                {
                    series.Add(new PieSeries<int>
                    {
                        Values = new[] { stats.RoomDistribution[i].Value },
                        Name = stats.RoomDistribution[i].Name,
                        Fill = new SolidColorPaint(SKColor.Parse(colors[i % colors.Length])),
                        InnerRadius = 60
                    });
                }
                PieChartRooms.Series = series.ToArray();
            }
        }

        private async void UploadBorder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    string filePath = files[0];
                    if (filePath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        UploadStatusText.Text = $"Uploading {System.IO.Path.GetFileName(filePath)}...";
                        UploadProgressBar.Visibility = Visibility.Visible;
                        UploadProgressBar.Value = 50; // Simulate progress
                        
                        try
                        {
                            var response = await _apiService.UploadFileAsync(filePath);
                            UploadProgressBar.Value = 100;
                            UploadStatusText.Text = "Upload complete!";
                            UpdateDashboard(response.Stats);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Upload failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            UploadStatusText.Text = "Click or drag and drop to upload";
                        }
                        finally
                        {
                            await Task.Delay(2000); // Hide progress after delay
                            UploadProgressBar.Visibility = Visibility.Collapsed;
                            UploadProgressBar.Value = 0;
                        }
                    }
                }
            }
        }
    }
}
