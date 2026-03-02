using System.Windows;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class VehicleEditWindow : Window
    {
        private readonly CarSharingDbContext _context;
        private readonly Vehicle? _existing; // null = добавление, not null = редактирование

        public VehicleEditWindow(CarSharingDbContext context, Vehicle? vehicle)
        {
            InitializeComponent();
            _context = context;
            _existing = vehicle;

            if (_existing != null)
            {
                // Режим редактирования — заполняем поля
                TxtTitle.Text = "Редактировать автомобиль";
                TxtNumber.Text = _existing.Number;
                TxtModel.Text = _existing.Model;
                TxtYear.Text = _existing.Year.ToString();
                TxtColor.Text = _existing.Color;
                SetComboValue(CmbFuel, _existing.Type_of_fuel);
                SetComboValue(CmbTransmission, _existing.Transmission);
                SetComboValue(CmbDrive, _existing.Drive);
                SetComboValue(CmbStatus, _existing.Status);
            }
            else
            {
                // Режим добавления — статус по умолчанию
                CmbStatus.SelectedIndex = 0; // Доступно
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(TxtNumber.Text) ||
                string.IsNullOrWhiteSpace(TxtModel.Text) ||
                string.IsNullOrWhiteSpace(TxtYear.Text) ||
                string.IsNullOrWhiteSpace(TxtColor.Text) ||
                CmbFuel.SelectedItem == null ||
                CmbTransmission.SelectedItem == null ||
                CmbDrive.SelectedItem == null ||
                CmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtYear.Text, out int year) || year < 1970 || year > 2100)
            {
                MessageBox.Show("Введите корректный год выпуска (1970–2100).", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка уникальности номера
            var numberInput = TxtNumber.Text.Trim();
            bool numberExists = _context.Vehicles.Any(v =>
                v.Number == numberInput &&
                (_existing == null || v.ID_Vehicles != _existing.ID_Vehicles));

            if (numberExists)
            {
                MessageBox.Show("Автомобиль с таким гос. номером уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_existing == null)
            {
                // Добавление нового
                var vehicle = new Vehicle
                {
                    Number       = numberInput,
                    Model        = TxtModel.Text.Trim(),
                    Type_of_fuel = GetComboValue(CmbFuel),
                    Year         = year,
                    Color        = TxtColor.Text.Trim(),
                    Transmission = GetComboValue(CmbTransmission),
                    Drive        = GetComboValue(CmbDrive),
                    Status       = GetComboValue(CmbStatus)
                };
                _context.Vehicles.Add(vehicle);
            }
            else
            {
                // Обновление существующего
                _existing.Number       = numberInput;
                _existing.Model        = TxtModel.Text.Trim();
                _existing.Type_of_fuel = GetComboValue(CmbFuel);
                _existing.Year         = year;
                _existing.Color        = TxtColor.Text.Trim();
                _existing.Transmission = GetComboValue(CmbTransmission);
                _existing.Drive        = GetComboValue(CmbDrive);
                _existing.Status       = GetComboValue(CmbStatus);
                _context.Vehicles.Update(_existing);
            }

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Вспомогательные методы для ComboBox
        private string GetComboValue(System.Windows.Controls.ComboBox cmb)
            => (cmb.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "";

        private void SetComboValue(System.Windows.Controls.ComboBox cmb, string value)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in cmb.Items)
            {
                if (item.Content?.ToString() == value)
                {
                    cmb.SelectedItem = item;
                    return;
                }
            }
        }
    }
}
