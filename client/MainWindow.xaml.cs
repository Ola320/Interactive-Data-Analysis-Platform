﻿using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using DataAnalizer.Views; // Ważne, aby widział folder Views


namespace DataAnalizer
{
    public partial class MainWindow : Window
    {
        // Przechowujemy oryginalną zawartość (Dashboard), żeby móc do niej wrócić
        private object _dashboardContent;

        public MainWindow()
        {
            InitializeComponent();
            // Zapamiętujemy, co było w MainContent na starcie (nasz Dashboard)
            _dashboardContent = MainContent.Content;
        }

        // Kliknięcie w Diagrams - podmieniamy widok
        private void BtnDiagrams_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DiagramsView();
            TxtHeader.Text = "Diagrams Analysis";
        }

        // Kliknięcie w Dashboard - przywracamy zapamiętany widok
        private void ShowDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _dashboardContent;
            TxtHeader.Text = "Dashboard";
        }

        // Kliknięcie w Settings - podmieniamy widok na ustawienia
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
            TxtHeader.Text = "Settings";
        }
    }
}