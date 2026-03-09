using System.Windows;
using System.Windows.Input;
using Car_sharingDB.Data;

namespace Car_sharingDB.Views
{
    public partial class LoginWindow : Window
    {
        private readonly CarSharingDbContext _context;

        public LoginWindow(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e) => TryLogin();

        private void PbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) TryLogin();
        }

        private void TryLogin()
        {
            var username = TxtUsername.Text.Trim();
            var password = PbPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль.");
                return;
            }

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user == null)
            {
                ShowError("Неверный логин или пароль.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void ShowError(string msg)
        {
            TxtError.Text = msg;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}
