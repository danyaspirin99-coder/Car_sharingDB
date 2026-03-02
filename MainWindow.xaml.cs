using System.Windows;
using Car_sharingDB.Data;
using Car_sharingDB.Views;

namespace Car_sharingDB
{
    public partial class MainWindow : Window
    {
        private readonly CarSharingDbContext _context;

        public MainWindow(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;

            // Открываем раздел автомобилей по умолчанию
            MainFrame.Navigate(new VehiclesPage(_context));
            HighlightButton(BtnVehicles);
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new VehiclesPage(_context));
            HighlightButton(BtnVehicles);
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            // TODO: MainFrame.Navigate(new ClientsPage(_context));
            MessageBox.Show("Раздел 'Клиенты' в разработке", "Информация");
        }

        private void BtnRent_Click(object sender, RoutedEventArgs e)
        {
            // TODO: MainFrame.Navigate(new RentPage(_context));
            MessageBox.Show("Раздел 'Аренда' в разработке", "Информация");
        }

        private void BtnPayments_Click(object sender, RoutedEventArgs e)
        {
            // TODO: MainFrame.Navigate(new PaymentsPage(_context));
            MessageBox.Show("Раздел 'Платежи' в разработке", "Информация");
        }

        private void HighlightButton(System.Windows.Controls.Button active)
        {
            var buttons = new[]
            {
                BtnVehicles, BtnClients, BtnRent, BtnPayments
            };
            foreach (var btn in buttons)
            {
                btn.Background = System.Windows.Media.Brushes.Transparent;
                btn.Foreground = System.Windows.Media.Brushes.LightGray;
            }
            active.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#3D5166"));
            active.Foreground = System.Windows.Media.Brushes.White;
        }
    }
}
