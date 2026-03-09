using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Car_sharingDB.Data;
using Car_sharingDB.Views;

namespace Car_sharingDB
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(args.Exception.ToString(),
                    "Ошибка UI", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            var services = new ServiceCollection();

            var connectionString =
                "Server=WIN-POAPFFPD7NG\\SQLEXPRESS;" +
                "Database=Car_sharingDB;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";

            services.AddDbContext<CarSharingDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            try
            {
                var context = ServiceProvider.GetRequiredService<CarSharingDbContext>();
                context.Database.OpenConnection();
                context.Database.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД:\n\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            var login = ServiceProvider.GetRequiredService<LoginWindow>();
            bool? result = login.ShowDialog();

            if (result == true)
            {
                var main = ServiceProvider.GetRequiredService<MainWindow>();
                // Назначаем главное окно явно — чтобы приложение не закрылось
                Application.Current.MainWindow = main;
                main.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
