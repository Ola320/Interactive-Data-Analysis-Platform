using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        // Obsługa naciśnięcia klawisza Enter w formularzu logowania
        private void LoginPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement == UsernameBox)
                {
                    e.Handled = true;
                    PasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == PasswordBox)
                {
                    e.Handled = true;
                    Login_Click(this, new RoutedEventArgs());
                }
            }
        }

        // Obsługa naciśnięcia klawisza Enter w formularzu rejestracji
        private void RegisterPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (Keyboard.FocusedElement == RegUsernameBox)
                {
                    RegEmailBox.Focus();
                }
                else if (Keyboard.FocusedElement == RegEmailBox)
                {
                    RegPasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == RegPasswordBox)
                {
                    RegConfirmPasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == RegConfirmPasswordBox)
                {
                    Register_Click(this, new RoutedEventArgs());
                }
            }
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
                
                await System.Threading.Tasks.Task.Delay(1000); 
                
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.ShowMainView();

                ClearForm();
            }
            else
            {
                ShowStatus("Błędny login lub hasło.", false);
                PasswordBox.Clear();
            }
        }

        private void ClearForm()
        {
            PasswordBox.Clear();
            
            if (RememberMeCheck.IsChecked != true)
            {
                UsernameBox.Clear();
            }

            HideStatus();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            HideStatus();

            var username = RegUsernameBox.Text.Trim();
            var email = RegEmailBox.Text.Trim();
            var password = RegPasswordBox.Password;
            var confirmPassword = RegConfirmPasswordBox.Password;

            // --- WALIDACJA ---
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

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
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

            // --- REJESTRACJA W API ---
            var registerSuccess = await _apiService.RegisterAsync(username, password, email); 
            
            if (registerSuccess)
            {
                ShowStatus("Konto utworzone! Trwa automatyczne logowanie...", true);
                
                RegUsernameBox.Clear();
                RegEmailBox.Clear();
                RegPasswordBox.Clear();
                RegConfirmPasswordBox.Clear();

                // --- AUTOMATYCZNE LOGOWANIE ---
                var loginSuccess = await _apiService.LoginAsync(username, password);
                
                if (loginSuccess)
                {
                    await System.Threading.Tasks.Task.Delay(1000); 

                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow?.ShowMainView();

                    ClearForm();
                }
                else
                {
                    ShowStatus("Konto utworzone, ale automatyczne logowanie się nie powiodło. Zaloguj się ręcznie.", false);
                    SwitchToLogin_Click(this, new RoutedEventArgs());
                }
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
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#bbf7d0"));
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#15803d"));
            }
            else
            {
                StatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fee2e2"));
                StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b91c1c"));
            }
        }

        private void HideStatus()
        {
            StatusBorder.Visibility = Visibility.Collapsed;
            StatusText.Text = "";
        }
    }
}