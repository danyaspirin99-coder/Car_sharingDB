using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharingDB.Models
{
    [Table("Rent")]
    public class Rent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Rent { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата начала")]
        public DateTime Beginnig_date { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата окончания")]
        public DateTime END_date { get; set; }

        // Внешние ключи
        [Required]
        [ForeignKey("Client")]
        [Display(Name = "Клиент")]
        public int ID_Client { get; set; }

        [Required]
        [ForeignKey("Vehicle")]
        [Display(Name = "Автомобиль")]
        public int ID_Vehicles { get; set; }

        // Навигационные свойства
        [Required]
        public virtual Client Client { get; set; }

        [Required]
        public virtual Vehicle Vehicle { get; set; }

        // Проверка дат
        public bool IsValidDates()
        {
            return END_date > Beginnig_date;
        }

        // Проверка активности аренды
        public bool IsActive()
        {
            return DateTime.Today >= Beginnig_date && DateTime.Today <= END_date;
        }
    }
}