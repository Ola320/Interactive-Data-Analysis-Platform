using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataAnalizer.Services;

namespace DataAnalizer.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new ApiService();

        // --- ZMIENNE DO BINDOWANIA (Zastępują bezpośrednie odwołania do XAML) ---
        private string _titleText = "Zaloguj się do platformy";
        private Visibility _loginPanelVisibility = Visibility.Visible;
        private Visibility _registerPanelVisibility = Visibility.Collapsed;

        private Visibility _statusVisibility = Visibility.Collapsed;
        private string _statusText;
        private SolidColorBrush _statusBackground;
        private SolidColorBrush _statusForeground;

        private string _loginUsername;
        private string _regUsername;
        private string _regEmail;

        // --- WŁAŚCIWOŚCI PUBLICZNE ---
        public string TitleText { get => _titleText; set { _titleText = value; OnPropertyChanged(); } }
        public Visibility LoginPanelVisibility { get => _loginPanelVisibility; set { _loginPanelVisibility = value; OnPropertyChanged(); } }
        public Visibility RegisterPanelVisibility { get => _registerPanelVisibility; set { _registerPanelVisibility = value; OnPropertyChanged(); } }

        public Visibility StatusVisibility { get => _statusVisibility; set { _statusVisibility = value; OnPropertyChanged(); } }
        public string StatusText { get => _statusText; set { _statusText = value; OnPropertyChanged(); } }
        public SolidColorBrush StatusBackground { get => _statusBackground; set { _statusBackground = value; OnPropertyChanged(); } }
        public SolidColorBrush StatusForeground { get => _statusForeground; set { _statusForeground = value; OnPropertyChanged(); } }

        public string LoginUsername { get => _loginUsername; set { _loginUsername = value; OnPropertyChanged(); } }
        public string RegUsername { get => _regUsername; set { _regUsername = value; OnPropertyChanged(); } }
        public string RegEmail { get => _regEmail; set { _regEmail = value; OnPropertyChanged(); } }

        // --- KOMENDY ---
        public ICommand SwitchToRegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            SwitchToRegisterCommand = new RelayCommand(_ => SwitchToRegister());
            SwitchToLoginCommand = new RelayCommand(_ => SwitchToLogin());
            LoginCommand = new RelayCommand(ExecuteLogin);
            RegisterCommand = new RelayCommand(ExecuteRegister);
        }

        // --- LOGIKA PRZEŁĄCZANIA PANELI ---
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

        // --- LOGIKA LOGOWANIA ---
        private async void ExecuteLogin(object parameter)
        {
            HideStatus();
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrEmpty(LoginUsername) || string.IsNullOrEmpty(password))
            {
                ShowStatus("Wprowadź login i hasło.", false);
                return;
            }

            var success = await _apiService.LoginAsync(LoginUsername, password);
            if (success)
            {
                ShowStatus("Zalogowano pomyślnie! Ładowanie...", true);
                await System.Threading.Tasks.Task.Delay(1000);

                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.ShowMainView();
            }
            else
            {
                ShowStatus("Błędny login lub hasło.", false);
            }
        }

        // --- LOGIKA REJESTRACJI ---
        private async void ExecuteRegister(object parameter)
        {
            HideStatus();

            // W MVVM do przekazania dwóch haseł najprościej użyć tablicy obiektów przekazanej z XAML
            var passwordBoxes = parameter as object[];
            if (passwordBoxes == null || passwordBoxes.Length != 2) return;

            var password = (passwordBoxes[0] as PasswordBox)?.Password;
            var confirmPassword = (passwordBoxes[1] as PasswordBox)?.Password;

            if (string.IsNullOrEmpty(RegUsername) || string.IsNullOrEmpty(RegEmail) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
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

            if (password.Length < 6)
            {
                ShowStatus("Hasło musi składać się z co najmniej 6 znaków.", false);
                return;
            }

            if (password != confirmPassword)
            {
                ShowStatus("Hasła nie są identyczne!", false);
                return;
            }

            var success = await _apiService.RegisterAsync(RegUsername, password, RegEmail);
            if (success)
            {
                ShowStatus("Konto utworzone pomyślnie! Możesz się zalogować.", true);
                RegUsername = "";
                RegEmail = "";
                (passwordBoxes[0] as PasswordBox)?.Clear();
                (passwordBoxes[1] as PasswordBox)?.Clear();
            }
            else
            {
                ShowStatus("Rejestracja nieudana. Użytkownik lub e-mail może już istnieć.", false);
            }
        }

        // --- LOGIKA KOMUNIKATÓW ---
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

        private void HideStatus()
        {
            StatusVisibility = Visibility.Collapsed;
            StatusText = "";
        }

        // --- MVVM ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}