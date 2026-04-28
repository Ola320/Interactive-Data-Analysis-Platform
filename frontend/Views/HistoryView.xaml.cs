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

        private async void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LogEntry log)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    // Find the Dashboard view in MainWindow
                    var dashboard = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.FindName("MainContentControl") as ContentControl;
                    
                    // Actually, MainWindow has a private field _dashboardView. 
                    // Let's use a better way to access it. 
                    // I will modify MainWindow to have a public method or property.
                    
                    mainWindow.ShowDashboardWithLog(log.Id);
                }
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LogEntry log)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{log.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _apiService.DeleteLogAsync(log.Id);
                        await LoadLogsAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting log: {ex.Message}");
                    }
                }
            }
        }

        private async void BtnRename_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is LogEntry log)
            {
                // Simple rename logic - for now using a MessageBox prompt isn't possible for input,
                // so I'll use a very basic Window in code for input.
                var inputWindow = new Window
                {
                    Title = "Rename Log",
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow,
                    ResizeMode = ResizeMode.NoResize
                };

                var stackPanel = new StackPanel { Margin = new Thickness(10) };
                var textBox = new TextBox { Text = log.Name, Margin = new Thickness(0, 0, 0, 10) };
                var saveButton = new Button { Content = "Save", IsDefault = true };
                
                saveButton.Click += async (s, ev) =>
                {
                    try
                    {
                        await _apiService.RenameLogAsync(log.Id, textBox.Text);
                        inputWindow.Close();
                        await LoadLogsAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error renaming log: {ex.Message}");
                    }
                };

                stackPanel.Children.Add(new TextBlock { Text = "Enter new name:", Margin = new Thickness(0, 0, 0, 5) });
                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(saveButton);
                inputWindow.Content = stackPanel;
                inputWindow.ShowDialog();
            }
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
