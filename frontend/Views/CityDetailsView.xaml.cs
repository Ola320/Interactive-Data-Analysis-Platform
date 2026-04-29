using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAnalizer.Services;

namespace DataAnalizer.Views
{
    public partial class CityDetailsView : UserControl
    {
        private readonly ApiService _apiService;

        public CityDetailsView()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            var city = TxtSearchCity.Text.Trim();
            if (string.IsNullOrEmpty(city) || city == "Enter city name...") return;

            try
            {
                TxtError.Visibility = Visibility.Collapsed;
                ResultsPanel.Visibility = Visibility.Collapsed;

                // For this example, we get the latest log ID. In a real app, you might let the user choose the log.
                var logs = await _apiService.GetLogsAsync();
                if (!logs.Any())
                {
                    TxtError.Text = "No data uploaded yet. Please upload a CSV on the dashboard.";
                    TxtError.Visibility = Visibility.Visible;
                    return;
                }

                int latestLogId = logs.First().Id;
                var details = await _apiService.GetCityDetailsAsync(latestLogId, city);

                if (details != null)
                {
                    TxtResultCityName.Text = details.City;
                    TxtResultTotalListings.Text = details.TotalListings.ToString("N0");
                    TxtResultAvgPrice.Text = $"${details.AvgPrice:N0}";
                    TxtResultAvgPriceSqm.Text = $"${details.AvgPricePerSqm:N0}";
                    ResultsPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    TxtError.Text = "City not found in the dataset.";
                    TxtError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                TxtError.Text = $"Error fetching data: {ex.Message}";
                TxtError.Visibility = Visibility.Visible;
            }
        }
    }
}
