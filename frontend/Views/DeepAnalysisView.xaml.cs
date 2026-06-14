using System.Windows.Controls;
using DataAnalizer.ViewModels;

namespace DataAnalizer.Views
{
    public partial class DeepAnalysisView : UserControl
    {
        public DeepAnalysisView()
        {
            InitializeComponent();
            this.DataContext = new DeepAnalysisViewModel();
        }
    }
}