using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Car_sharingDB.Data;
using CarSharingDB.Models;

namespace Car_sharingDB.Views
{
    public partial class ClientsPage : Page
    {
        private readonly CarSharingDbContext _context;
        private List<Client> _allClients = new();

        public ClientsPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadClients();
        }

        private void LoadClients()
        {
            _allClients = _context.Clients.ToList();
            ClientsGrid.ItemsSource = _allClients;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = TxtSearch.Text.ToLower().Trim();
            ClientsGrid.ItemsSource = string.IsNullOrEmpty(q)
                ? _allClients
                : _allClients.Where(c =>
                    c.Surname.ToLower().Contains(q) ||
                    c.Name.ToLower().Contains(q) ||
                    c.Phone_number.ToLower().Contains(q) ||
                    c.License.ToLower().Contains(q)).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new ClientEditWindow(_context, null);
            if (dlg.ShowDialog() == true) LoadClients();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is not Client selected)
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var dlg = new ClientEditWindow(_context, selected);
            if (dlg.ShowDialog() == true) LoadClients();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is not Client selected)
            {
                MessageBox.Show("Выберите клиента для удаления.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool hasRent = _context.Rents.Any(r => r.ID_Client == selected.ID_Client);
            if (hasRent)
            {
                MessageBox.Show("Нельзя удалить клиента с активной арендой.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить клиента «{selected.Surname} {selected.Name}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _context.Clients.Remove(selected);
                _context.SaveChanges();
                LoadClients();
            }
        }
    }
}
