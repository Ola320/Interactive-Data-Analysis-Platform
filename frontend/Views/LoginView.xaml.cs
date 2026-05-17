using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataAnalizer.Services;

namespace DataAnalizer.Views
{
    public partial class LoginView : UserControl
    {
        private readonly ApiService _apiService = new ApiService();

        public LoginView()
        {
            InitializeComponent();
        }

        private void SwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Stwórz nowe konto";
            LoginPanel.Visibility = Visibility.Collapsed;
            RegisterPanel.Visibility = Visibility.Visible;
            HideStatus();
        }

        private void SwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            TitleText.Text = "Zaloguj się do platformy";
            RegisterPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            HideStatus();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            HideStatus();
            var username = UsernameBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowStatus("Wprowadź login i hasło.", false);
                return;
            }

            var success = await _apiService.LoginAsync(username, password);
            if (success)
            {
                ShowStatus("Zalogowano pomyślnie! Ładowanie...", true);
                
                // Opóźnienie dla efektu wizualnego komunikatu sukcesu
                await System.Threading.Tasks.Task.Delay(1000); 
                
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.ShowMainView();
            }
            else
            {
                ShowStatus("Błędny login lub hasło.", false);
            }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            HideStatus();

            var username = RegUsernameBox.Text.Trim();
            var email = RegEmailBox.Text.Trim();
            var password = RegPasswordBox.Password;
            var confirmPassword = RegConfirmPasswordBox.Password;

            // --- WALIDACJA PO STRONIE KLIENTA (WPF) ---
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowStatus("Wszystkie pola są wymagane!", false);
                return;
            }

            if (username.Length < 3)
            {
                ShowStatus("Nazwa użytkownika musi mieć min. 3 znaki.", false);
                return;
            }

            // Walidacja Email regular expression
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                ShowStatus("Podany adres e-mail jest nieprawidłowy.", false);
                return;
            }

            // Walidacja siły hasła (np. min 6 znaków)
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

            // --- ZAPYTANIE DO API ---
            var success = await _apiService.RegisterAsync(username, password, email); 
            
            if (success)
            {
                ShowStatus("Konto utworzone pomyślnie! Możesz się zalogować.", true);
                // Czyszczenie pól rejestracji
                RegUsernameBox.Clear();
                RegEmailBox.Clear();
                RegPasswordBox.Clear();
                RegConfirmPasswordBox.Clear();
            }
            else
            {
                ShowStatus("Rejestracja nieudana. Użytkownik lub e-mail może już istnieć.", false);
            }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            StatusText.Text = message;
            StatusBorder.Visibility = Visibility.Visible;

            if (isSuccess)
            {
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbf7d0")); // jasnozielony
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#15803d")); // ciemnozielony
            }
            else
            {
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fee2e2")); // jasnoczerwony
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b91c1c")); // ciemnoczerwony
            }
        }

        private void HideStatus()
        {
            StatusBorder.Visibility = Visibility.Collapsed;
            StatusText.Text = "";
        }
    }
}