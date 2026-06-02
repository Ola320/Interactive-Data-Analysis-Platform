using System.Windows.Controls;
using DataAnalizer.ViewModels;

namespace DataAnalizer.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }
    }
}