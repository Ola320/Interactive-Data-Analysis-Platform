using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAnalizer.ViewModels;

namespace DataAnalizer.Views
{
    public partial class LoginView : UserControl
    {
        private readonly LoginViewModel _viewModel;
        
        private bool _isSyncingLoginPwd = false;
        private bool _isSyncingRegPwd = false;
        private bool _isSyncingRegConfPwd = false;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            this.DataContext = _viewModel;

            _viewModel.OnLoginSuccess = () =>
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.ShowMainView();
                
                PasswordBox.Clear();
                PasswordTextBox.Clear();
                RegPasswordBox.Clear();
                RegPasswordTextBox.Clear();
                RegConfirmPasswordBox.Clear();
                RegConfirmPasswordTextBox.Clear();
            };
        }

        public void ResetToLoginView()
        {
            _viewModel.ResetToLoginView();
            PasswordBox.Clear();
            PasswordTextBox.Clear();
            RegPasswordBox.Clear();
            RegPasswordTextBox.Clear();
            RegConfirmPasswordBox.Clear();
            RegConfirmPasswordTextBox.Clear();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingLoginPwd) return;
            _isSyncingLoginPwd = true;
            _viewModel.Password = PasswordBox.Password;
            PasswordTextBox.Text = PasswordBox.Password;
            _isSyncingLoginPwd = false;
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingLoginPwd) return;
            _isSyncingLoginPwd = true;
            _viewModel.Password = PasswordTextBox.Text;
            PasswordBox.Password = PasswordTextBox.Text;
            _isSyncingLoginPwd = false;
        }

        private void RegPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingRegPwd) return;
            _isSyncingRegPwd = true;
            _viewModel.RegPassword = RegPasswordBox.Password;
            RegPasswordTextBox.Text = RegPasswordBox.Password;
            _viewModel.UpdatePasswordStrength(RegPasswordBox.Password);
            _isSyncingRegPwd = false;
        }

        private void RegPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingRegPwd) return;
            _isSyncingRegPwd = true;
            _viewModel.RegPassword = RegPasswordTextBox.Text;
            RegPasswordBox.Password = RegPasswordTextBox.Text;
            _viewModel.UpdatePasswordStrength(RegPasswordTextBox.Text);
            _isSyncingRegPwd = false;
        }

        private void RegConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isSyncingRegConfPwd) return;
            _isSyncingRegConfPwd = true;
            _viewModel.RegConfirmPassword = RegConfirmPasswordBox.Password;
            RegConfirmPasswordTextBox.Text = RegConfirmPasswordBox.Password;
            _isSyncingRegConfPwd = false;
        }

        private void RegConfirmPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isSyncingRegConfPwd) return;
            _isSyncingRegConfPwd = true;
            _viewModel.RegConfirmPassword = RegConfirmPasswordTextBox.Text;
            RegConfirmPasswordBox.Password = RegConfirmPasswordTextBox.Text;
            _isSyncingRegConfPwd = false;
        }

        private void LoginPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement == UsernameBox)
                {
                    e.Handled = true;
                    if (_viewModel.IsPasswordVisible) PasswordTextBox.Focus();
                    else PasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == PasswordBox || Keyboard.FocusedElement == PasswordTextBox)
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
                {
                    if (_viewModel.IsRegPasswordVisible) RegPasswordTextBox.Focus();
                    else RegPasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == RegPasswordBox || Keyboard.FocusedElement == RegPasswordTextBox)
                {
                    if (_viewModel.IsRegConfirmPasswordVisible) RegConfirmPasswordTextBox.Focus();
                    else RegConfirmPasswordBox.Focus();
                }
                else if (Keyboard.FocusedElement == RegConfirmPasswordBox || Keyboard.FocusedElement == RegConfirmPasswordTextBox)
                {
                    if (_viewModel.RegisterCommand.CanExecute(null))
                        _viewModel.RegisterCommand.Execute(null);
                }
            }
        }
    }
}