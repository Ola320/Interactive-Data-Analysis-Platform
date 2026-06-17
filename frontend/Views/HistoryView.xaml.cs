using DataAnalizer.Models;
using DataAnalizer.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataAnalizer.Views
{
    public partial class HistoryView : UserControl
    {
        private readonly HistoryViewModel _viewModel;

        public HistoryView()
        {
            InitializeComponent();

            _viewModel = new HistoryViewModel(
                showDashboardAsync: async logId =>
                {
                    if (Application.Current.MainWindow
                        is MainWindow mainWindow)
                    {
                        await mainWindow
                            .ShowDashboardWithLogAsync(logId);
                    }
                },

                confirmDelete: log =>
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            $"Czy na pewno chcesz usunąć „{log.Name}”?",
                            "Potwierdzenie usunięcia",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning
                        );

                    return result == MessageBoxResult.Yes;
                }
            );

            DataContext = _viewModel;

            Loaded += HistoryView_Loaded;
        }

        private async void HistoryView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await LoadLogsAsync();
        }

        

        

        private void BtnRename_Click(object sender, RoutedEventArgs e)

        {
            if (sender is not Button button ||
                button.DataContext is not LogEntry log)
            {
                return;
            }

            var inputWindow = new Window
            {
                Title = "Zmiana nazwy",
                Width = 300,
                Height = 160,
                WindowStartupLocation =
                    WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            var textBox = new TextBox
            {
                Text = log.Name,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var saveButton = new Button
            {
                Content = "Zapisz",
                IsDefault = true
            };

            saveButton.Click += async (_, _) =>
            {
                string newName = textBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(newName))
                {
                    MessageBox.Show(
                        "Nazwa nie może być pusta.",
                        "Błąd walidacji",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );

                    return;
                }

                if (string.Equals(
                        newName,
                        log.Name,
                        StringComparison.Ordinal))
                {
                    inputWindow.Close();
                    return;
                }

                try
                {
                    saveButton.IsEnabled = false;

                    await _viewModel.RenameLogAsync(
                        log.Id,
                        newName
                    );

                    inputWindow.Close();
                }
                catch (Exception ex)
                {
                    saveButton.IsEnabled = true;

                    MessageBox.Show(
                        $"Nie udało się zmienić nazwy:\n{ex.Message}",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            };

            stackPanel.Children.Add(
                new TextBlock
                {
                    Text = "Wprowadź nową nazwę:",
                    Margin = new Thickness(0, 0, 0, 5)
                }
            );

            stackPanel.Children.Add(textBox);
            stackPanel.Children.Add(saveButton);

            inputWindow.Content = stackPanel;

            inputWindow.Loaded += (_, _) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };

            inputWindow.ShowDialog();
        }

        public async Task LoadLogsAsync()
        {
            try
            {
                await _viewModel.LoadLogsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Nie udało się pobrać historii:\n{ex.Message}",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}