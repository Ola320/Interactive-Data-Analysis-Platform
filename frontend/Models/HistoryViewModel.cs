using DataAnalizer.Commands;
using DataAnalizer.Models;
using DataAnalizer.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataAnalizer.ViewModels
{
    public class HistoryViewModel
    {
        private readonly ApiService _apiService;

        private readonly Func<int, Task> _showDashboardAsync;
        private readonly Func<LogEntry, bool> _confirmDelete;

        public ObservableCollection<LogEntry> Logs { get; }
            = new ObservableCollection<LogEntry>();

        public ICommand AnalyzeCommand { get; }

        public ICommand DeleteCommand { get; }

        public HistoryViewModel(
            Func<int, Task> showDashboardAsync,
            Func<LogEntry, bool> confirmDelete)
        {
            _apiService = new ApiService();

            _showDashboardAsync = showDashboardAsync;
            _confirmDelete = confirmDelete;

            AnalyzeCommand =
                new AsyncRelayCommand<LogEntry>(
                    AnalyzeAsync,
                    log => log is not null
                );

            DeleteCommand =
                new AsyncRelayCommand<LogEntry>(
                    DeleteAsync,
                    log => log is not null
                );
        }

        public async Task LoadLogsAsync()
        {
            var logs = await _apiService.GetLogsAsync();

            Logs.Clear();

            foreach (LogEntry log in logs)
            {
                Logs.Add(log);
            }
        }

        private async Task AnalyzeAsync(LogEntry? log)
        {
            if (log is null)
                return;

            AppState.CurrentLogId = log.Id;

            await _showDashboardAsync(log.Id);
        }

        private async Task DeleteAsync(LogEntry? log)
        {
            if (log is null)
                return;

            bool confirmed = _confirmDelete(log);

            if (!confirmed)
                return;

            await _apiService.DeleteLogAsync(log.Id);

            await LoadLogsAsync();
        }

        public async Task RenameLogAsync(
            int logId,
            string newName)
        {
            await _apiService.RenameLogAsync(
                logId,
                newName
            );

            await LoadLogsAsync();
        }
    }
}