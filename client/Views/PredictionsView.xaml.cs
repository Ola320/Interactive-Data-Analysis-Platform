using System.Windows.Controls;

namespace DataAnalizer.Views
{
    public partial class PredictionsView : UserControl
    {
        public PredictionsView()
        {
            InitializeComponent();
            CalculatePrice();
        }

        private void UpdatePrice_Event(object sender, SelectionChangedEventArgs e) => CalculatePrice();
        private void UpdatePrice_Event(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) => CalculatePrice();

        private void CalculatePrice()
        {
            if (TxtEstimatedPrice == null) return;

            // Bardzo uproszczony model matematyczny:
            // Bazowa cena za m2 (np. 12 000 PLN) * metraż + bonusy za pokoje/łazienki
            double area = SliderArea.Value;
            double rooms = SliderRooms.Value;
            double baths = SliderBaths.Value;
            
            double basePrice = area * 11500;
            double roomBonus = rooms * 15000;
            double bathBonus = baths * 25000;

            double totalPrice = basePrice + roomBonus + bathBonus;

            TxtEstimatedPrice.Text = $"{totalPrice:N0} PLN";
        }
    }
}