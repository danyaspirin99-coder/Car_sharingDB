using System.Windows;
using System.Windows.Controls;
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
            Navigate(BtnVehicles, new VehiclesPage(_context));
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
            => Navigate(BtnVehicles, new VehiclesPage(_context));
        private void BtnClients_Click(object sender, RoutedEventArgs e)
            => Navigate(BtnClients, new ClientsPage(_context));
        private void BtnRent_Click(object sender, RoutedEventArgs e)
            => Navigate(BtnRent, new RentPage(_context));
        private void BtnPayments_Click(object sender, RoutedEventArgs e)
            => Navigate(BtnPayments, new PaymentsPage(_context));
        private void BtnAccessories_Click(object sender, RoutedEventArgs e)
            => Navigate(BtnAccessories, new AccessoriesPage(_context));

        private void Navigate(Button active, object page)
        {
            MainFrame.Navigate(page);
            var buttons = new[] { BtnVehicles, BtnClients, BtnRent, BtnPayments, BtnAccessories };
            foreach (var btn in buttons)
                btn.Style = (Style)FindResource("TopNavButton");
            active.Style = (Style)FindResource("TopNavButtonActive");
        }
    }
}
