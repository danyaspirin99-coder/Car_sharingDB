using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class RentPage : Page
    {
        private readonly CarSharingDbContext _context;

        public RentPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadRents();
        }

        private void LoadRents()
        {
            var rents = _context.Rents
                .Include(r => r.Client)
                .Include(r => r.Vehicle)
                .ToList();

            RentGrid.ItemsSource = rents.Select(r => new RentViewModel
            {
                ID_Rent = r.ID_Rent,
                ClientName = $"{r.Client.Surname} {r.Client.Name}",
                VehicleInfo = $"{r.Vehicle.Model} ({r.Vehicle.Number})",
                Beginnig_date = r.Beginnig_date,
                END_date = r.END_date,
                RentStatus = r.END_date >= DateTime.Today ? "Активна" : "Завершена"
            }).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new RentEditWindow(_context);
            if (dlg.ShowDialog() == true) LoadRents();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (RentGrid.SelectedItem is not RentViewModel selected)
            {
                MessageBox.Show("Выберите аренду для закрытия.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selected.RentStatus == "Завершена")
            {
                MessageBox.Show("Эта аренда уже завершена.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Закрыть аренду для «{selected.ClientName}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            var rent = _context.Rents.Find(selected.ID_Rent);
            if (rent != null)
            {
                rent.END_date = DateTime.Today;
                var vehicle = _context.Vehicles.Find(rent.ID_Vehicles);
                if (vehicle != null) vehicle.Status = "Доступно";
                _context.SaveChanges();
                LoadRents();
            }
        }
    }

    // ViewModel для отображения в DataGrid
    public class RentViewModel
    {
        public int ID_Rent { get; set; }
        public string ClientName { get; set; } = "";
        public string VehicleInfo { get; set; } = "";
        public DateTime Beginnig_date { get; set; }
        public DateTime END_date { get; set; }
        public string RentStatus { get; set; } = "";
    }
}
