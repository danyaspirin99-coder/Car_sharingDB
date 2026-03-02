using Microsoft.EntityFrameworkCore;
using CarSharingDB.Models;

namespace Car_sharingDB.Data
{
    public class CarSharingDbContext : DbContext
    {
        public CarSharingDbContext(DbContextOptions<CarSharingDbContext> options)
            : base(options)
        {
        }

        // DbSet для каждой таблицы
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Accessory> Accessories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ClientPayments> ClientPayments { get; set; }
        public DbSet<VehiclesAccessories> VehiclesAccessories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальные индексы для Client
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Phone_number).IsUnique();
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.License).IsUnique();

            // Уникальный индекс для Vehicles
            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.Number).IsUnique();

            // Связи для Rent
            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Client)
                .WithMany(c => c.Rents)
                .HasForeignKey(r => r.ID_Client)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rent>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Rents)
                .HasForeignKey(r => r.ID_Vehicles)
                .OnDelete(DeleteBehavior.Restrict);

            // Уникальные ограничения для Rent (один клиент — одна аренда, одна машина — одна аренда)
            modelBuilder.Entity<Rent>()
                .HasIndex(r => r.ID_Client).IsUnique();
            modelBuilder.Entity<Rent>()
                .HasIndex(r => r.ID_Vehicles).IsUnique();

            // Проверка дат аренды
            modelBuilder.Entity<Rent>()
                .HasCheckConstraint("CHK_Rent_Dates", "END_date > Beginnig_date");

            // Связи для ClientPayments
            modelBuilder.Entity<ClientPayments>()
                .HasOne(cp => cp.Client)
                .WithMany(c => c.ClientPayments)
                .HasForeignKey(cp => cp.ID_Client)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientPayments>()
                .HasOne(cp => cp.Payment)
                .WithMany(p => p.ClientPayments)
                .HasForeignKey(cp => cp.ID_Payments)
                .OnDelete(DeleteBehavior.Cascade);

            // Связи для VehiclesAccessories
            modelBuilder.Entity<VehiclesAccessories>()
                .HasOne(va => va.Vehicle)
                .WithMany(v => v.VehiclesAccessories)
                .HasForeignKey(va => va.ID_Vehicles)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VehiclesAccessories>()
                .HasOne(va => va.Accessory)
                .WithMany(a => a.VehiclesAccessories)
                .HasForeignKey(va => va.ID_Accessories)
                .OnDelete(DeleteBehavior.Cascade);

            // Уникальная пара машина-аксессуар
            modelBuilder.Entity<VehiclesAccessories>()
                .HasIndex(va => new { va.ID_Vehicles, va.ID_Accessories }).IsUnique();
        }
    }
}
