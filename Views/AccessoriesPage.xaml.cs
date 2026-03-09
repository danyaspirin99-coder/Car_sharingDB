using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class AccessoriesPage : Page
    {
        private readonly CarSharingDbContext _context;
        private List<AccessoryViewModel> _allItems = new();

        public AccessoriesPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadVehicleFilter();
            LoadAccessories();
        }

        private void LoadVehicleFilter()
        {
            var items = new List<VehicleItem> { new() { ID_Vehicles = 0, DisplayInfo = "Все автомобили" } };
            items.AddRange(_context.Vehicles.ToList().Select(v => new VehicleItem
            {
                ID_Vehicles = v.ID_Vehicles,
                DisplayInfo = $"{v.Model} ({v.Number})"
            }));
            CmbVehicleFilter.ItemsSource = items;
            CmbVehicleFilter.SelectedIndex = 0;
        }

        private void LoadAccessories()
        {
            _allItems = _context.VehiclesAccessories
                .Include(va => va.Vehicle)
                .Include(va => va.Accessory)
                .ToList()
                .Select(va => new AccessoryViewModel
                {
                    ID_Vehicles_Accessories = va.ID_Vehicles_Accessories,
                    ID_Vehicles = va.ID_Vehicles,
                    VehicleInfo = $"{va.Vehicle.Model} ({va.Vehicle.Number})",
                    AccessoryName = va.Accessory.Name
                }).ToList();

            AccessoriesGrid.ItemsSource = _allItems;
        }

        private void CmbVehicleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbVehicleFilter.SelectedItem is not VehicleItem selected) return;

            AccessoriesGrid.ItemsSource = selected.ID_Vehicles == 0
                ? _allItems
                : _allItems.Where(a => a.ID_Vehicles == selected.ID_Vehicles).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new AccessoryAddWindow(_context);
            if (dlg.ShowDialog() == true)
            {
                LoadAccessories();
                LoadVehicleFilter();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (AccessoriesGrid.SelectedItem is not AccessoryViewModel selected)
            {
                MessageBox.Show("Выберите запись для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить аксессуар «{selected.AccessoryName}» у «{selected.VehicleInfo}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            var item = _context.VehiclesAccessories.Find(selected.ID_Vehicles_Accessories);
            if (item != null)
            {
                _context.VehiclesAccessories.Remove(item);
                _context.SaveChanges();
                LoadAccessories();
            }
        }
    }

    public class AccessoryViewModel
    {
        public int ID_Vehicles_Accessories { get; set; }
        public int ID_Vehicles { get; set; }
        public string VehicleInfo { get; set; } = "";
        public string AccessoryName { get; set; } = "";
    }
}
