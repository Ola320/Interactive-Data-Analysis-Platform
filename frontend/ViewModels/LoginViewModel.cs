using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DataAnalizer.Services;

namespace DataAnalizer.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new ApiService();

        // Zdarzenie do informowania widoku, że logowanie się udało (aby zmienić okno)
        public Action? OnLoginSuccess { get; set; }

        #region Właściwości (Properties)
        private string _titleText = "Zaloguj się do platformy";
        public string TitleText { get => _titleText; set { _titleText = value; OnPropertyChanged(); } }

        private Visibility _loginPanelVisibility = Visibility.Visible;
        public Visibility LoginPanelVisibility { get => _loginPanelVisibility; set { _loginPanelVisibility = value; OnPropertyChanged(); } }

        private Visibility _registerPanelVisibility = Visibility.Collapsed;
        public Visibility RegisterPanelVisibility { get => _registerPanelVisibility; set { _registerPanelVisibility = value; OnPropertyChanged(); } }

        // Dane Logowania
        private string _username = string.Empty;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        public string Password { get; set; } = string.Empty;

        private bool _rememberMe;
        public bool RememberMe { get => _rememberMe; set { _rememberMe = value; OnPropertyChanged(); } }

        // Dane Rejestracji
        private string _regUsername = string.Empty;
        public string RegUsername { get => _regUsername; set { _regUsername = value; OnPropertyChanged(); } }

        private string _regEmail = string.Empty;
        public string RegEmail { get => _regEmail; set { _regEmail = value; OnPropertyChanged(); } }

        public string RegPassword { get; set; } = string.Empty;
        public string RegConfirmPassword { get; set; } = string.Empty;

        // Status 
        private string _statusText = string.Empty;
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }

        private Visibility _statusVisibility = Visibility.Collapsed;
        public Visibility StatusVisibility { get => _statusVisibility; set { _statusVisibility = value; OnPropertyChanged(); } }

        private Brush _statusBackground = Brushes.Transparent;
        public Brush StatusBackground { get => _statusBackground; set { _statusBackground = value; OnPropertyChanged(); } }

        private Brush _statusForeground = Brushes.Black;
        public Brush StatusForeground { get => _statusForeground; set { _statusForeground = value; OnPropertyChanged(); } }
        #endregion

        #region Komendy (Commands)
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        #endregion

        public LoginViewModel()
        {
            SwitchToRegisterCommand = new RelayCommand(_ => SwitchToRegister());
            SwitchToLoginCommand = new RelayCommand(_ => SwitchToLogin());
            LoginCommand = new RelayCommand(async _ => await ExecuteLogin());
            RegisterCommand = new RelayCommand(async _ => await ExecuteRegister());
        }

        private void SwitchToRegister()
        {
            TitleText = "Stwórz nowe konto";
            LoginPanelVisibility = Visibility.Collapsed;
            RegisterPanelVisibility = Visibility.Visible;
            HideStatus();
        }

        private void SwitchToLogin()
        {
            TitleText = "Zaloguj się do platformy";
            RegisterPanelVisibility = Visibility.Collapsed;
            LoginPanelVisibility = Visibility.Visible;
            HideStatus();
        }

        public void ClearForm()
        {
            if (!RememberMe) Username = string.Empty;
            Password = string.Empty;
            RegUsername = string.Empty;
            RegEmail = string.Empty;
            RegPassword = string.Empty;
            RegConfirmPassword = string.Empty;
            HideStatus();
        }

        public void ResetToLoginView()
        {
            SwitchToLogin();
            ClearForm();
        }

        private async Task ExecuteLogin()
        {
            HideStatus();

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ShowStatus("Wprowadź login i hasło.", false);
                return;
            }

            var success = await _apiService.LoginAsync(Username, Password);
            if (success)
            {
                ShowStatus("Zalogowano pomyślnie! Ładowanie...", true);
                await Task.Delay(1000);
                
                // Wywołaj akcję zmiany widoku w MainWindow
                OnLoginSuccess?.Invoke();
                ClearForm();
            }
            else
            {
                ShowStatus("Błędny login lub hasło.", false);
            }
        }

        private async Task ExecuteRegister()
        {
            HideStatus();

            if (string.IsNullOrEmpty(RegUsername) || string.IsNullOrEmpty(RegEmail) || 
                string.IsNullOrEmpty(RegPassword) || string.IsNullOrEmpty(RegConfirmPassword))
            {
                ShowStatus("Wszystkie pola są wymagane!", false);
                return;
            }

            if (RegUsername.Length < 3)
            {
                ShowStatus("Nazwa użytkownika musi mieć min. 3 znaki.", false);
                return;
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(RegEmail))
            {
                ShowStatus("Podany adres e-mail jest nieprawidłowy.", false);
                return;
            }

            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&_#^+-]).{8,}$");
            if (!passwordRegex.IsMatch(RegPassword))
            {
                ShowStatus("Hasło musi mieć min. 8 znaków, zawierać wielką i małą literę, cyfrę oraz znak specjalny.", false);
                return;
            }

            if (RegPassword != RegConfirmPassword)
            {
                ShowStatus("Hasła nie są identyczne!", false);
                return;
            }

            var registerSuccess = await _apiService.RegisterAsync(RegUsername, RegPassword, RegEmail); 
            if (registerSuccess)
            {
                ShowStatus("Konto utworzone! Trwa automatyczne logowanie...", true);
                RegUsername = string.Empty; RegEmail = string.Empty;

                var loginSuccess = await _apiService.LoginAsync(RegUsername, RegPassword);
                if (loginSuccess)
                {
                    await Task.Delay(1000); 
                    OnLoginSuccess?.Invoke();
                    ClearForm();
                }
                else
                {
                    ShowStatus("Konto utworzone, ale logowanie nie powiodło się. Zaloguj się ręcznie.", false);
                    SwitchToLogin();
                }
            }
            else
            {
                ShowStatus("Rejestracja nieudana. Użytkownik lub e-mail może już istnieć.", false);
            }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            StatusText = message;
            StatusVisibility = Visibility.Visible;
            if (isSuccess)
            {
                StatusBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbf7d0"));
                StatusForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#15803d"));
            }
            else
            {
                StatusBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fee2e2"));
                StatusForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b91c1c"));
            }
        }

        private void HideStatus() => StatusVisibility = Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}