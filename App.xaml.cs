using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Car_sharingDB.Data;

namespace Car_sharingDB
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            // === СТРОКА ПОДКЛЮЧЕНИЯ ===
            // Замени "YourServerName" на имя своего SQL Server
            // Например: "DESKTOP-ABC123\\SQLEXPRESS" или просто "localhost"
            var connectionString =
                "Server=WIN-POAPFFPD7NG\\SQLEXPRESS;" +
                "Database=Car_sharingDB;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";

            // Регистрация DbContext
            services.AddDbContext<CarSharingDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Регистрация главного окна
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();

            // Запуск главного окна через DI
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
