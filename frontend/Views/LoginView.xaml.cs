using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAnalizer.ViewModels;

namespace DataAnalizer.Views
{
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            this.DataContext = _viewModel;

            // Kiedy logowanie się powiedzie, ViewModel poinformuje nas tutaj, 
            // a my bezpiecznie przełączymy okno z poziomu View.
            _viewModel.OnLoginSuccess = () =>
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.ShowMainView();
                
                // Po pomyślnym zalogowaniu resetujemy pola haseł (UI)
                PasswordBox.Clear();
                RegPasswordBox.Clear();
                RegConfirmPasswordBox.Clear();
            };
        }

        public void ResetToLoginView()
        {
            _viewModel.ResetToLoginView();
            PasswordBox.Clear();
            RegPasswordBox.Clear();
            RegConfirmPasswordBox.Clear();
        }

        // Aktualizacja haseł w ViewModel (dla bezpieczeństwa PasswordBox nie wspiera Bindingu bez modyfikacji)
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) 
            => _viewModel.Password = PasswordBox.Password;

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e) 
            => _viewModel.RegPassword = RegPasswordBox.Password;

        private void RegConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e) 
            => _viewModel.RegConfirmPassword = RegConfirmPasswordBox.Password;

        // Obsługa naciśnięcia Enter
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
                    if (_viewModel.LoginCommand.CanExecute(null))
                        _viewModel.LoginCommand.Execute(null);
                }
            }
        }

        private void RegisterPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (Keyboard.FocusedElement == RegUsernameBox)
                    RegEmailBox.Focus();
                else if (Keyboard.FocusedElement == RegEmailBox)
                    RegPasswordBox.Focus();
                else if (Keyboard.FocusedElement == RegPasswordBox)
                    RegConfirmPasswordBox.Focus();
                else if (Keyboard.FocusedElement == RegConfirmPasswordBox)
                {
                    if (_viewModel.RegisterCommand.CanExecute(null))
                        _viewModel.RegisterCommand.Execute(null);
                }
            }
        }
    }
}