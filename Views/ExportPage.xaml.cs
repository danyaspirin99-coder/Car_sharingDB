using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ClosedXML.Excel;
using Car_sharingDB.Data;

namespace Car_sharingDB.Views
{
    public partial class ExportPage : Page
    {
        private readonly CarSharingDbContext _context;

        public ExportPage(CarSharingDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadCounts();
        }

        private void LoadCounts()
        {
            TxtVehiclesCount.Text    = $"{_context.Vehicles.Count()} записей";
            TxtClientsCount.Text     = $"{_context.Clients.Count()} записей";
            TxtRentCount.Text        = $"{_context.Rents.Count()} записей";
            TxtPaymentsCount.Text    = $"{_context.Payments.Count()} записей";
            TxtAccessoriesCount.Text = $"{_context.VehiclesAccessories.Count()} записей";
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            if (ChkVehicles.IsChecked != true && ChkClients.IsChecked != true &&
                ChkRent.IsChecked != true && ChkPayments.IsChecked != true &&
                ChkAccessories.IsChecked != true)
            {
                MessageBox.Show("Выберите хотя бы одну таблицу.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Title = "Сохранить отчёт",
                FileName = $"CarSharing_Export_{DateTime.Now:yyyy-MM-dd}",
                Filter = "Excel файл (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx"
            };

            if (dlg.ShowDialog() != true) return;

            try
            {
                using var wb = new XLWorkbook();

                // Стиль заголовков
                var headerColor = XLColor.FromHtml("#1A8A6E");
                var altRowColor = XLColor.FromHtml("#F0FBF8");

                // ===== АВТОМОБИЛИ =====
                if (ChkVehicles.IsChecked == true)
                {
                    var ws = wb.AddWorksheet("Автомобили");
                    var headers = new[] { "ID", "Гос. номер", "Модель", "Топливо", "Год", "Цвет", "КПП", "Привод", "Статус" };
                    WriteHeaders(ws, headers, headerColor);

                    var data = _context.Vehicles.ToList();
                    for (int i = 0; i < data.Count; i++)
                    {
                        var v = data[i];
                        var row = i + 2;
                        ws.Cell(row, 1).Value = v.ID_Vehicles;
                        ws.Cell(row, 2).Value = v.Number;
                        ws.Cell(row, 3).Value = v.Model;
                        ws.Cell(row, 4).Value = v.Type_of_fuel;
                        ws.Cell(row, 5).Value = v.Year;
                        ws.Cell(row, 6).Value = v.Color;
                        ws.Cell(row, 7).Value = v.Transmission;
                        ws.Cell(row, 8).Value = v.Drive;
                        ws.Cell(row, 9).Value = v.Status;
                        if (i % 2 == 1) ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = altRowColor;
                    }
                    AutoFit(ws, headers.Length);
                }

                // ===== КЛИЕНТЫ =====
                if (ChkClients.IsChecked == true)
                {
                    var ws = wb.AddWorksheet("Клиенты");
                    var headers = new[] { "ID", "Фамилия", "Имя", "Отчество", "Дата рождения", "Телефон", "Вод. удостоверение" };
                    WriteHeaders(ws, headers, headerColor);

                    var data = _context.Clients.ToList();
                    for (int i = 0; i < data.Count; i++)
                    {
                        var c = data[i];
                        var row = i + 2;
                        ws.Cell(row, 1).Value = c.ID_Client;
                        ws.Cell(row, 2).Value = c.Surname;
                        ws.Cell(row, 3).Value = c.Name;
                        ws.Cell(row, 4).Value = c.Father_name;
                        ws.Cell(row, 5).Value = c.Date_of_birth.ToString("dd.MM.yyyy");
                        ws.Cell(row, 6).Value = c.Phone_number;
                        ws.Cell(row, 7).Value = c.License;
                        if (i % 2 == 1) ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = altRowColor;
                    }
                    AutoFit(ws, headers.Length);
                }

                // ===== АРЕНДА =====
                if (ChkRent.IsChecked == true)
                {
                    var ws = wb.AddWorksheet("Аренда");
                    var headers = new[] { "ID", "Клиент", "Автомобиль", "Дата начала", "Дата окончания", "Статус" };
                    WriteHeaders(ws, headers, headerColor);

                    var data = _context.Rents
                        .Include(r => r.Client)
                        .Include(r => r.Vehicle)
                        .ToList();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var r = data[i];
                        var row = i + 2;
                        var clientName = $"{r.Client?.Surname} {r.Client?.Name}".Trim();
                        var vehicleInfo = $"{r.Vehicle?.Model} ({r.Vehicle?.Number})";
                        var status = r.END_date < DateTime.Today ? "Завершена" : "Активна";
                        ws.Cell(row, 1).Value = r.ID_Rent;
                        ws.Cell(row, 2).Value = clientName;
                        ws.Cell(row, 3).Value = vehicleInfo;
                        ws.Cell(row, 4).Value = r.Beginnig_date.ToString("dd.MM.yyyy");
                        ws.Cell(row, 5).Value = r.END_date.ToString("dd.MM.yyyy");
                        ws.Cell(row, 6).Value = status;
                        if (i % 2 == 1) ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = altRowColor;
                    }
                    AutoFit(ws, headers.Length);
                }

                // ===== ПЛАТЕЖИ =====
                if (ChkPayments.IsChecked == true)
                {
                    var ws = wb.AddWorksheet("Платежи");
                    var headers = new[] { "ID", "Клиент", "Тип оплаты", "Способ оплаты", "Сумма" };
                    WriteHeaders(ws, headers, headerColor);

                    var data = _context.ClientPayments
                        .Include(cp => cp.Client)
                        .Include(cp => cp.Payment)
                        .ToList();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var cp = data[i];
                        var row = i + 2;
                        var clientName = $"{cp.Client?.Surname} {cp.Client?.Name}".Trim();
                        ws.Cell(row, 1).Value = cp.Payment?.ID_Payments;
                        ws.Cell(row, 2).Value = clientName;
                        ws.Cell(row, 3).Value = cp.Type_of_payment;
                        ws.Cell(row, 4).Value = cp.Payment?.Way_of_payment;
                        ws.Cell(row, 5).Value = (double)(cp.Payment?.Amount ?? 0);
                        ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00 ₽";
                        if (i % 2 == 1) ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = altRowColor;
                    }

                    AutoFit(ws, headers.Length);
                }

                // ===== АКСЕССУАРЫ =====
                if (ChkAccessories.IsChecked == true)
                {
                    var ws = wb.AddWorksheet("Аксессуары");
                    var headers = new[] { "ID", "Аксессуар", "Автомобиль", "Гос. номер" };
                    WriteHeaders(ws, headers, headerColor);

                    var data = _context.VehiclesAccessories
                        .Include(va => va.Vehicle)
                        .Include(va => va.Accessory)
                        .ToList();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var va = data[i];
                        var row = i + 2;
                        ws.Cell(row, 1).Value = va.ID_Vehicles_Accessories;
                        ws.Cell(row, 2).Value = va.Accessory?.Name;
                        ws.Cell(row, 3).Value = va.Vehicle?.Model;
                        ws.Cell(row, 4).Value = va.Vehicle?.Number;
                        if (i % 2 == 1) ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = altRowColor;
                    }
                    AutoFit(ws, headers.Length);
                }

                wb.SaveAs(dlg.FileName);

                TxtStatus.Visibility = Visibility.Visible;
                TxtStatus.Text = $"✅ Файл успешно сохранён: {Path.GetFileName(dlg.FileName)}";

                if (MessageBox.Show("Файл сохранён! Открыть его?", "Готово",
                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void WriteHeaders(IXLWorksheet ws, string[] headers, XLColor color)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = color;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        private static void AutoFit(IXLWorksheet ws, int colCount)
        {
            for (int i = 1; i <= colCount; i++)
                ws.Column(i).AdjustToContents();
        }
    }
}
