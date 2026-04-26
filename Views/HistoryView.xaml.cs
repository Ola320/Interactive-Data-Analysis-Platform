using System;
using System.Windows;
using System.Windows.Controls;
using DataAnalizer.Services;

namespace DataAnalizer.Views
{
    public partial class HistoryView : UserControl
    {
        private readonly ApiService _apiService;

        public HistoryView()
        {
            InitializeComponent();
            _apiService = new ApiService();
            _ = LoadLogsAsync();
        }

        private async System.Threading.Tasks.Task LoadLogsAsync()
        {
            try
            {
                var logs = await _apiService.GetLogsAsync();
                LogsDataGrid.ItemsSource = logs;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
