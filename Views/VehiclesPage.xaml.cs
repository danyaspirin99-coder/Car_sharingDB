using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class VehiclesPage : Page
    {
        private readonly CarSharingDbContext _context;
        private List<Vehicle> _allVehicles = new();

        public VehiclesPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadVehicles();
        }

        // Загрузка всех автомобилей из БД
        private void LoadVehicles()
        {
            _allVehicles = _context.Vehicles.ToList();
            VehiclesGrid.ItemsSource = _allVehicles;
        }

        // Поиск по модели или номеру
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = TxtSearch.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(query))
            {
                VehiclesGrid.ItemsSource = _allVehicles;
            }
            else
            {
                VehiclesGrid.ItemsSource = _allVehicles
                    .Where(v => v.Model.ToLower().Contains(query) ||
                                v.Number.ToLower().Contains(query))
                    .ToList();
            }
        }

        // Добавить автомобиль
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VehicleEditWindow(_context, null);
            if (dialog.ShowDialog() == true)
                LoadVehicles();
        }

        // Редактировать выбранный автомобиль
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Выберите автомобиль для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new VehicleEditWindow(_context, selected);
            if (dialog.ShowDialog() == true)
                LoadVehicles();
        }

        // Удалить выбранный автомобиль
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem is not Vehicle selected)
            {
                MessageBox.Show("Выберите автомобиль для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка — есть ли активная аренда
            bool hasRent = _context.Rents.Any(r => r.ID_Vehicles == selected.ID_Vehicles);
            if (hasRent)
            {
                MessageBox.Show("Нельзя удалить автомобиль с активной арендой.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить автомобиль «{selected.Model} ({selected.Number})»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _context.Vehicles.Remove(selected);
                _context.SaveChanges();
                LoadVehicles();
            }
        }
    }
}
