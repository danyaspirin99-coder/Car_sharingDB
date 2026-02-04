using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace CarSharingDB.Models
{
    [Table("Vehicles")]
    public class Vehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Vehicles { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Гос. номер")]
        public string Number { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Тип топлива")]
        public string Type_of_fuel { get; set; }

        [Required]
        [Range(1970, 2100)]
        [Display(Name = "Год выпуска")]
        public int Year { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Цвет")]
        public string Color { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Коробка передач")]
        public string Transmission { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Привод")]
        public string Drive { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Статус")]
        public string Status { get; set; }

        // Навигационные свойства
        public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
        public virtual ICollection<VehiclesAccessories> VehiclesAccessories { get; set; } = new List<VehiclesAccessories>();
    }
}