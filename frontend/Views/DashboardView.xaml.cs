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

        public async Task LoadLogById(int logId)
        {
            try
            {
                var stats = await _apiService.GetLogDetailsAsync(logId);
                UpdateDashboard(stats);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load analysis: {ex.Message}");
            }
        }

        private void UpdateDashboard(AnalyticsData stats)
        {
            if (stats == null || stats.Summary == null) return;

            // Update KPIs
            TxtTotalOffers.Text = stats.Summary.TotalOffers.ToString("N0");
            TxtAvgPrice.Text = $"${stats.Summary.AvgPrice:N0}";
            TxtMedianPrice.Text = $"${stats.Summary.MedianPrice:N0}";
            TxtAvgPricePerSqm.Text = $"${stats.Summary.AvgPricePerSqm:N0}";

            // Update Bar Chart (Top Cities)
            if (stats.Charts?.CityChart != null)
            {
                BarChartCities.Series = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = stats.Charts.CityChart.Select(c => c.PricePerSqm).ToArray(),
                        Fill = new SolidColorPaint(SKColor.Parse("#4f46e5")),
                        MaxBarWidth = 30,
                        YToolTipLabelFormatter = (chartPoint) => $"{chartPoint.PrimaryValue:N0} zł/m²"
                    }
                };

                BarChartCities.XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = stats.Charts.CityChart.Select(c => c.Name).ToArray(),
                        LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748b"))
                    }
                };
            }

            // Update Pie Chart (Room Distribution)
            if (stats.Charts?.RoomsChart != null)
            {
                var colors = new[] { "#4f46e5", "#6366f1", "#818cf8", "#a5b4fc", "#c7d2fe" };
                var series = new List<ISeries>();
                for (int i = 0; i < stats.Charts.RoomsChart.Count; i++)
                {
                    series.Add(new PieSeries<int>
                    {
                        Values = new[] { stats.Charts.RoomsChart[i].Value },
                        Name = stats.Charts.RoomsChart[i].Name,
                        Fill = new SolidColorPaint(SKColor.Parse(colors[i % colors.Length])),
                        InnerRadius = 60
                    });
                }
                PieChartRooms.Series = series.ToArray();
            }

            // Update Trend Chart
            if (stats.Charts?.Trends != null)
            {
                LineChartTrends.Series = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = stats.Charts.Trends.Select(t => t.AvgPrice).ToArray(),
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColor.Parse("#4f46e5")) { StrokeThickness = 3 },
                        GeometrySize = 10,
                        GeometryFill = new SolidColorPaint(SKColor.Parse("#4f46e5"))
                    }
                };

                LineChartTrends.XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = stats.Charts.Trends.Select(t => t.Year.ToString()).ToArray(),
                        LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748b"))
                    }
                };
            }

            // Update Scatter Chart (Price vs Distance)
            if (stats.Charts?.PriceVsDistance != null)
            {
                ScatterChartDistance.Series = new ISeries[]
                {
                    new ScatterSeries<PriceDistance>
                    {
                        Values = stats.Charts.PriceVsDistance.ToArray(),
                        Mapping = (point, index) => new(point.Distance, point.Price),
                        Fill = new SolidColorPaint(SKColor.Parse("#818cf8")) { Opacity = 0.6f },
                        MinGeometrySize = 5,
                        MaxGeometrySize = 15
                    }
                };

                ScatterChartDistance.XAxes = new Axis[] { new Axis { Name = "Distance (km)" } };
                ScatterChartDistance.YAxes = new Axis[] { new Axis { Name = "Price (zł)" } };
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
