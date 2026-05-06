using System.Windows;
using DataAnalizer.Views;

namespace DataAnalizer
{
    public partial class MainWindow : Window
    {
        private DashboardView _dashboardView;
        private HistoryView _historyView;
        private CityDetailsView _cityDetailsView;
        private LoginView _loginView;

        public MainWindow()
        {
            InitializeComponent();
            _dashboardView = new DashboardView();
            _historyView = new HistoryView();
            _cityDetailsView = new CityDetailsView();
            _loginView = new LoginView();

            // Load default view
            MainContentControl.Content = _loginView;
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _dashboardView;
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _historyView;
        }

        private void BtnCityLookup_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _cityDetailsView;
        }

        // Metoda do przełączania na główny widok po zalogowaniu
        public void ShowMainView()
        {
            MainContentControl.Content = _dashboardView;
        }
    }
}