using System;
using System.Linq;
using System.Windows;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class ClientEditWindow : Window
    {
        private readonly CarSharingDbContext _context;
        private readonly Client? _existing;

        public ClientEditWindow(CarSharingDbContext context, Client? client)
        {
            InitializeComponent();
            _context = context;
            _existing = client;

            if (_existing != null)
            {
                TxtTitle.Text = "Редактировать клиента";
                TxtSurname.Text = _existing.Surname;
                TxtName.Text = _existing.Name;
                TxtFatherName.Text = _existing.Father_name;
                DpBirthDate.SelectedDate = _existing.Date_of_birth;
                TxtPhone.Text = _existing.Phone_number;
                TxtLicense.Text = _existing.License;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSurname.Text) ||
                string.IsNullOrWhiteSpace(TxtName.Text) ||
                DpBirthDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TxtPhone.Text) ||
                string.IsNullOrWhiteSpace(TxtLicense.Text))
            {
                MessageBox.Show("Заполните все обязательные поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка уникальности телефона
            var phone = TxtPhone.Text.Trim();
            if (_context.Clients.Any(c => c.Phone_number == phone &&
                (_existing == null || c.ID_Client != _existing.ID_Client)))
            {
                MessageBox.Show("Клиент с таким номером телефона уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка уникальности водительского удостоверения
            var license = TxtLicense.Text.Trim();
            if (_context.Clients.Any(c => c.License == license &&
                (_existing == null || c.ID_Client != _existing.ID_Client)))
            {
                MessageBox.Show("Клиент с таким водительским удостоверением уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_existing == null)
            {
                var client = new Client
                {
                    Surname = TxtSurname.Text.Trim(),
                    Name = TxtName.Text.Trim(),
                    Father_name = string.IsNullOrWhiteSpace(TxtFatherName.Text) ? null : TxtFatherName.Text.Trim(),
                    Date_of_birth = DpBirthDate.SelectedDate!.Value,
                    Phone_number = phone,
                    License = license
                };
                _context.Clients.Add(client);
            }
            else
            {
                _existing.Surname = TxtSurname.Text.Trim();
                _existing.Name = TxtName.Text.Trim();
                _existing.Father_name = string.IsNullOrWhiteSpace(TxtFatherName.Text) ? null : TxtFatherName.Text.Trim();
                _existing.Date_of_birth = DpBirthDate.SelectedDate!.Value;
                _existing.Phone_number = phone;
                _existing.License = license;
                _context.Clients.Update(_existing);
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
    }
}
