using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DataAnalizer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object? _currentView;
        private bool _isLoggedIn;
        private string _username;

        public event EventHandler<object>? OnViewChanged;

        public object? CurrentView
        {
            get => _currentView;
            set 
            { 
                _currentView = value; 
                OnPropertyChanged(); 
                OnViewChanged?.Invoke(this, value!);
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set 
            { 
                _isLoggedIn = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(IsLoggedOut));
            }
        }

        public bool IsLoggedOut => !IsLoggedIn;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            _username = string.Empty;
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(Logout);
            IsLoggedIn = false;
        }

        private void Navigate(object? destination)
        {
            if (destination?.ToString() == "Login")
            {
                CurrentView = "Login";
            }
        }

        public void LoginSuccess(string username)
        {
            Username = username;
            IsLoggedIn = true;
        }

        private void Logout(object? obj)
        {
            Username = string.Empty;
            IsLoggedIn = false;
            CurrentView = "Login";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        public RelayCommand(Action<object?> execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}