using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class RentEditWindow : Window
    {
        private readonly CarSharingDbContext _context;

        public RentEditWindow(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadComboBoxes();
            DpStart.SelectedDate = DateTime.Today;
            DpEnd.SelectedDate = DateTime.Today.AddDays(1);
        }

        private void LoadComboBoxes()
        {
            var busyClientIds = _context.Rents.Select(r => r.ID_Client).ToHashSet();
            CmbClient.ItemsSource = _context.Clients
                .ToList()
                .Where(c => !busyClientIds.Contains(c.ID_Client))
                .Select(c => new ClientItem
                {
                    ID_Client = c.ID_Client,
                    FullName = $"{c.Surname} {c.Name} {c.Father_name}".Trim()
                }).ToList();

            CmbVehicle.ItemsSource = _context.Vehicles
                .Where(v => v.Status == "Доступно")
                .ToList()
                .Select(v => new VehicleItem
                {
                    ID_Vehicles = v.ID_Vehicles,
                    DisplayInfo = $"{v.Model} ({v.Number})"
                }).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClient.SelectedItem is not ClientItem client ||
                CmbVehicle.SelectedItem is not VehicleItem vehicle ||
                DpStart.SelectedDate == null || DpEnd.SelectedDate == null ||
                CmbPaymentWay.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtAmount.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpEnd.SelectedDate <= DpStart.SelectedDate)
            {
                MessageBox.Show("Дата окончания должна быть позже даты начала.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtAmount.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаём аренду
            var rent = new Rent
            {
                ID_Client = client.ID_Client,
                ID_Vehicles = vehicle.ID_Vehicles,
                Beginnig_date = DpStart.SelectedDate.Value,
                END_date = DpEnd.SelectedDate.Value
            };
            _context.Rents.Add(rent);

            // Помечаем машину как забронированную
            var v = _context.Vehicles.Find(vehicle.ID_Vehicles);
            if (v != null) v.Status = "Забронировано";

            // Автоматически создаём платёж за аренду
            var payment = new Payment
            {
                Way_of_payment = ((ComboBoxItem)CmbPaymentWay.SelectedItem).Content.ToString()!,
                Amount = amount
            };
            _context.Payments.Add(payment);
            _context.SaveChanges();

            var clientPayment = new ClientPayments
            {
                ID_Client = client.ID_Client,
                ID_Payments = payment.ID_Payments,
                Type_of_payment = "Плата за аренду"
            };
            _context.ClientPayments.Add(clientPayment);

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

    public class ClientItem
    {
        public int ID_Client { get; set; }
        public string FullName { get; set; } = "";
    }

    public class VehicleItem
    {
        public int ID_Vehicles { get; set; }
        public string DisplayInfo { get; set; } = "";
    }
}
