using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataAnalizer.Views
{
    public partial class DiagramsView : UserControl
    {
        private Random _rnd = new Random();

        public DiagramsView()
        {
            InitializeComponent();
            // Zdarzenie Loaded zapewnia, że Canvas ma już obliczoną szerokość i wysokość (ActualWidth/Height)
            this.Loaded += (s, e) => DrawAllCharts();
        }

        private void DrawAllCharts()
        {
            DrawColumnChart();
            DrawBarChart();
            DrawLineChart();
        }

        /// <summary>
        /// Rysuje pionowy wykres słupkowy (pary słupków żółty/czerwony)
        /// </summary>
        private void DrawColumnChart()
        {
            ColumnCanvas.Children.Clear();
            double w = ColumnCanvas.ActualWidth;
            double h = ColumnCanvas.ActualHeight;

            // Rysowanie tła: Osi X, Y oraz poziomych linii pomocniczych
            AddAxis(ColumnCanvas, w, h);

            int count = 8; // Liczba grup słupków
            double groupWidth = w / count; // Szerokość miejsca na jedną grupę
            double barWidth = groupWidth * 0.35; // Szerokość pojedynczego słupka w grupie

            for (int i = 0; i < count; i++)
            {
                // Obliczanie pozycji startowej grupy na osi X
                double xPos = (i * groupWidth) + (groupWidth * 0.1); 
                
                // --- Słupek LEWY (Żółty/Pomarańczowy) ---
                double h1 = _rnd.Next(20, (int)h - 30);
                CreateRect(ColumnCanvas, xPos, h - h1, barWidth, h1, "#F1C40F");

                // --- Słupek PRAWY (Czerwony/Bordowy) ---
                double h2 = _rnd.Next(20, (int)h - 30);
                CreateRect(ColumnCanvas, xPos + barWidth, h - h2, barWidth, h2, "#C0392B");
            }
        }

        /// <summary>
        /// Rysuje poziomy wykres słupkowy (Single Bar Chart)
        /// </summary>
        private void DrawBarChart()
        {
            BarCanvas.Children.Clear();
            double w = BarCanvas.ActualWidth;
            double h = BarCanvas.ActualHeight;

            // Rysowanie tła (osie i siatka)
            AddAxis(BarCanvas, w, h);

            int count = 12; // Liczba poziomych pasków
            double barHeight = (h / count) * 0.6; // Wysokość paska względem dostępnego miejsca

            for (int i = 0; i < count; i++)
            {
                // Obliczanie pozycji Y (od góry do dołu)
                double yPos = i * (h / count) + (barHeight * 0.2);
                
                // Szerokość paska jest losowana (imitacja wartości danych)
                double barWidth = _rnd.Next(40, (int)w - 50);
                
                // Rysowanie paska (kolor złoty/pomarańczowy)
                CreateRect(BarCanvas, 0, yPos, barWidth, barHeight, "#F39C12");
            }
        }

        /// <summary>
        /// Rysuje wykres liniowy za pomocą łamanej Polyline
        /// </summary>
        private void DrawLineChart()
        {
            LineCanvas.Children.Clear();
            double w = LineCanvas.ActualWidth;
            double h = LineCanvas.ActualHeight;

            // Rysowanie tła (osie i siatka)
            AddAxis(LineCanvas, w, h);

            // Obiekt Polyline - jedna ciągła linia łącząca wiele punktów
            Polyline line = new Polyline
            {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980B9")), // Niebieski
                StrokeThickness = 2.5,
                StrokeLineJoin = PenLineJoin.Round // Zaokrąglone rogi połączeń linii
            };

            int pointsCount = 10;
            for (int i = 0; i < pointsCount; i++)
            {
                // X: Rozmieszczone równomiernie co 1/9 szerokości
                // Y: Losowa wartość wysokości (odwrócona, bo w WPF 0 jest na górze)
                double x = i * (w / (pointsCount - 1));
                double y = _rnd.Next(20, (int)h - 20);
                line.Points.Add(new Point(x, y));
            }

            LineCanvas.Children.Add(line);
        }

        /// <summary>
        /// METODA POMOCNICZA: Rysuje szkielet wykresu (Osie i linie siatki)
        /// </summary>
        private void AddAxis(Canvas canvas, double w, double h)
        {
            // --- RYSOWANIE OSI Y (Pionowa linia z lewej) ---
            Line axisY = new Line { X1 = 0, Y1 = 0, X2 = 0, Y2 = h, Stroke = Brushes.LightGray, StrokeThickness = 1.5 };
            
            // --- RYSOWANIE OSI X (Pozioma linia na dole) ---
            Line axisX = new Line { X1 = 0, Y1 = h, X2 = w, Y2 = h, Stroke = Brushes.LightGray, StrokeThickness = 1.5 };
            
            canvas.Children.Add(axisY);
            canvas.Children.Add(axisX);

            // --- RYSOWANIE SIATKI (Poziome linie pomocnicze w tle) ---
            int gridLines = 5;
            for (int i = 0; i < gridLines; i++)
            {
                double y = (h / gridLines) * i;
                Line gridLine = new Line 
                { 
                    X1 = 0, Y1 = y, X2 = w, Y2 = y, 
                    Stroke = new SolidColorBrush(Color.FromRgb(245, 245, 245)), // Bardzo jasny szary
                    StrokeThickness = 1 
                };
                canvas.Children.Add(gridLine);
            }
        }

        /// <summary>
        /// METODA POMOCNICZA: Tworzy prostokąt (słupek) i dodaje go do wybranego Canvasa
        /// </summary>
        private void CreateRect(Canvas canvas, double x, double y, double width, double height, string hexColor)
        {
            Rectangle r = new Rectangle
            {
                Width = width,
                Height = Math.Max(0, height), // Zabezpieczenie przed ujemną wysokością
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor))
            };

            // Ustawienie pozycji obiektu wewnątrz Canvas
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y);
            
            canvas.Children.Add(r);
        }
    }
}