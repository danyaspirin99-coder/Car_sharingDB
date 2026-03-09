using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class AccessoryAddWindow : Window
    {
        private readonly CarSharingDbContext _context;

        public AccessoryAddWindow(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            CmbVehicle.ItemsSource = _context.Vehicles.ToList()
                .Select(v => new VehicleItem
                {
                    ID_Vehicles = v.ID_Vehicles,
                    DisplayInfo = $"{v.Model} ({v.Number})"
                }).ToList();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbVehicle.SelectedItem is not VehicleItem vehicle ||
                CmbAccessory.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var accessoryName = ((ComboBoxItem)CmbAccessory.SelectedItem).Content.ToString()!;

            // Найти или создать аксессуар
            var accessory = _context.Accessories.FirstOrDefault(a => a.Name == accessoryName);
            if (accessory == null)
            {
                accessory = new Accessory { Name = accessoryName };
                _context.Accessories.Add(accessory);
                _context.SaveChanges();
            }

            // Проверка на дубликат
            bool exists = _context.VehiclesAccessories.Any(va =>
                va.ID_Vehicles == vehicle.ID_Vehicles &&
                va.ID_Accessories == accessory.ID_Accessories);

            if (exists)
            {
                MessageBox.Show("У этого автомобиля уже есть такой аксессуар.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _context.VehiclesAccessories.Add(new VehiclesAccessories
            {
                ID_Vehicles = vehicle.ID_Vehicles,
                ID_Accessories = accessory.ID_Accessories
            });
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
