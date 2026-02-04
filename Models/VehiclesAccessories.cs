using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharingDB.Models
{
    [Table("Vehicles_Accessories")]
    public class VehiclesAccessories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Vehicles_Accessories { get; set; }

        // Внешние ключи
        [Required]
        [ForeignKey("Vehicle")]
        public int ID_Vehicles { get; set; }

        [Required]
        [ForeignKey("Accessory")]
        public int ID_Accessories { get; set; }

        // Навигационные свойства
        [Required]
        public virtual Vehicle Vehicle { get; set; }

        [Required]
        public virtual Accessory Accessory { get; set; }

        // Переопределение ToString для удобства
        public override string ToString()
        {
            return $"{Vehicle?.Model} - {Accessory?.Name}";
        }
    }
}