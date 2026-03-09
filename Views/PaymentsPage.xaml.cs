using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Car_sharingDB.Data;

namespace Car_sharingDB.Views
{
    public partial class PaymentsPage : Page
    {
        private readonly CarSharingDbContext _context;

        public PaymentsPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadPayments();
        }

        private void LoadPayments()
        {
            var data = _context.ClientPayments
                .Include(cp => cp.Client)
                .Include(cp => cp.Payment)
                .ToList()
                .Select(cp => new
                {
                    cp.ID_Client_Payments,
                    ClientName = $"{cp.Client.Surname} {cp.Client.Name}",
                    cp.Type_of_payment,
                    cp.Payment.Way_of_payment,
                    cp.Payment.Amount
                }).ToList();

            PaymentsGrid.ItemsSource = data;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PaymentAddWindow(_context);
            if (dlg.ShowDialog() == true) LoadPayments();
        }
    }
}
