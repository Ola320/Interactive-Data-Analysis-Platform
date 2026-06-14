using System.Windows;
using DataAnalizer.Views;
using DataAnalizer.ViewModels;

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

            // Domyślny start na oknie logowania
            MainContentControl.Content = _loginView;
            ViewModel.CurrentView = _loginView;

            // Monitorowanie powrotu do ekranu logowania (np. po użyciu przycisku "Log out")
            ViewModel.OnViewChanged += (sender, newView) => {
                if (newView is string viewName && viewName == "Login") 
                {
                    MainContentControl.Content = _loginView;
                    
                    // Czyścimy formularz w LoginViewModel przy wylogowaniu
                    if (_loginView.DataContext is LoginViewModel loginVM)
                    {
                        loginVM.ResetToLoginView();
                    }
                }
            };
        }

        public void ShowDashboardWithLog(int logId)
        {
            MainContentControl.Content = _dashboardView;
            _ = _dashboardView.LoadLogById(logId);
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

        private void BtnDeepAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new Views.DeepAnalysisView();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = _profileView;
        }

        public void ShowMainView()
        {
            MainContentControl.Content = _dashboardView;
            
            // ZGODNE Z MVVM: Pobieranie nazwy użytkownika bezpośrednio z ViewModelu logowania
            string user = "Użytkownik";
            if (_loginView.DataContext is LoginViewModel loginViewModel)
            {
                // Sprawdzamy login z panelu logowania lub panelu rejestracji
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
        }
    }
}