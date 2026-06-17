using DataAnalizer.Models;
using DataAnalizer.ViewModels;
using DataAnalizer.Views;
using System.Threading.Tasks;
using System.Windows;

namespace DataAnalizer
{
    public partial class MainWindow : Window
    {
        private DashboardView _dashboardView;
        private HistoryView _historyView;
        private CityDetailsView _cityDetailsView;
        private LoginView _loginView;
        private ProfileView _profileView;

        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            
            _dashboardView = new DashboardView();
            _historyView = new HistoryView();
            _cityDetailsView = new CityDetailsView();
            _loginView = new LoginView();
            _profileView = new ProfileView();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

        
            MainContentControl.Content = _loginView;
            ViewModel.CurrentView = _loginView;

         
            ViewModel.OnViewChanged += (sender, newView) => {
                if (newView is string viewName && viewName == "Login") 
                {
                    MainContentControl.Content = _loginView;
                    
                 
                    if (_loginView.DataContext is LoginViewModel loginVM)
                    {
                        loginVM.ResetToLoginView();
                    }
                }
            };
        }

        public async Task ShowDashboardWithLogAsync(int logId)
        {
            MainContentControl.Content = _dashboardView;
            ViewModel.CurrentView = _dashboardView;

            await _dashboardView.LoadLogByIdAsync(logId);
        }

        private async void BtnDashboard_Click( object sender,
            RoutedEventArgs e)
        {
            MainContentControl.Content = _dashboardView;
            ViewModel.CurrentView = _dashboardView;

            if (AppState.CurrentLogId > 0)
            {
                await _dashboardView.LoadLogByIdAsync(
                    AppState.CurrentLogId
                );
            }
            else
            {
                await _dashboardView.LoadLatestDataAsync();
            }
        }

        private async void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _historyView;
            await _historyView.LoadLogsAsync();
        }

        private void BtnCityLookup_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _cityDetailsView;
        }

        private void BtnDeepAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new Views.DeepAnalysisView();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _profileView;
        }

        public async void ShowMainView()
        {
            MainContentControl.Content = _dashboardView;
            ViewModel.CurrentView = _dashboardView;

            string user = "Użytkownik";

            if (_loginView.DataContext is LoginViewModel loginViewModel)
            {
                if (!string.IsNullOrEmpty(loginViewModel.Username))
                {
                    user = loginViewModel.Username;
                }
                else if (!string.IsNullOrEmpty(loginViewModel.RegUsername))
                {
                    user = loginViewModel.RegUsername;
                }
            }

            ViewModel.LoginSuccess(user);

            await _dashboardView.LoadLatestDataAsync();
        }
    }
    
}