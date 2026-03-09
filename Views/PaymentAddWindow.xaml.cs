using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class PaymentAddWindow : Window
    {
        private readonly CarSharingDbContext _context;

        public PaymentAddWindow(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            CmbClient.ItemsSource = _context.Clients
                .ToList()
                .Select(c => new ClientItem
                {
                    ID_Client = c.ID_Client,
                    FullName = $"{c.Surname} {c.Name}".Trim()
                }).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClient.SelectedItem is not ClientItem client ||
                CmbType.SelectedItem == null ||
                CmbWay.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtAmount.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtAmount.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var payment = new Payment
            {
                Way_of_payment = ((ComboBoxItem)CmbWay.SelectedItem).Content.ToString()!,
                Amount = amount
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            var link = new ClientPayments
            {
                ID_Client = client.ID_Client,
                ID_Payments = payment.ID_Payments,
                Type_of_payment = ((ComboBoxItem)CmbType.SelectedItem).Content.ToString()!
            };
            _context.ClientPayments.Add(link);
            _context.SaveChanges();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
