using Microsoft.EntityFrameworkCore;
using CarSharingDB.Models;

namespace Car_sharingDB.Data
{
    /// <summary>
    /// Репозиторий для работы с данными каршеринга.
    /// Используй методы этого класса в своих окнах WPF.
    /// </summary>
    public class CarSharingRepository
    {
        private readonly CarSharingDbContext _context;

        public CarSharingRepository(CarSharingDbContext context)
        {
            _context = context;
        }

        // ===================== КЛИЕНТЫ =====================

        public List<Client> GetAllClients()
            => _context.Clients.ToList();

        public Client? GetClientById(int id)
            => _context.Clients.Find(id);

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void UpdateClient(Client client)
        {
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public void DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                _context.SaveChanges();
            }
        }

        // ===================== АВТОМОБИЛИ =====================

        public List<Vehicle> GetAllVehicles()
            => _context.Vehicles.ToList();

        public List<Vehicle> GetAvailableVehicles()
            => _context.Vehicles.Where(v => v.Status == "Доступно").ToList();

        public void AddVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
            }
        }

        // ===================== АРЕНДА =====================

        public List<Rent> GetAllRents()
            => _context.Rents
                .Include(r => r.Client)
                .Include(r => r.Vehicle)
                .ToList();

        public List<Rent> GetActiveRents()
            => _context.Rents
                .Include(r => r.Client)
                .Include(r => r.Vehicle)
                .Where(r => r.END_date >= DateTime.Today)
                .ToList();

        public void AddRent(Rent rent)
        {
            _context.Rents.Add(rent);
            // Помечаем машину как занятую
            var vehicle = _context.Vehicles.Find(rent.ID_Vehicles);
            if (vehicle != null) vehicle.Status = "Забронировано";
            _context.SaveChanges();
        }

        public void CloseRent(int rentId)
        {
            var rent = _context.Rents.Find(rentId);
            if (rent != null)
            {
                // Освобождаем машину
                var vehicle = _context.Vehicles.Find(rent.ID_Vehicles);
                if (vehicle != null) vehicle.Status = "Доступно";
                _context.Rents.Remove(rent);
                _context.SaveChanges();
            }
        }

        // ===================== ПЛАТЕЖИ =====================

        public List<ClientPayments> GetClientPayments(int clientId)
            => _context.ClientPayments
                .Include(cp => cp.Payment)
                .Where(cp => cp.ID_Client == clientId)
                .ToList();

        public void AddPayment(Payment payment, int clientId, string type)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();

            var link = new ClientPayments
            {
                ID_Client = clientId,
                ID_Payments = payment.ID_Payments,
                Type_of_payment = type
            };
            _context.ClientPayments.Add(link);
            _context.SaveChanges();
        }

        // ===================== АКСЕССУАРЫ =====================

        public List<Accessory> GetAllAccessories()
            => _context.Accessories.ToList();

        public List<VehiclesAccessories> GetVehicleAccessories(int vehicleId)
            => _context.VehiclesAccessories
                .Include(va => va.Accessory)
                .Where(va => va.ID_Vehicles == vehicleId)
                .ToList();
    }
}
