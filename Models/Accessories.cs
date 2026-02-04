using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharingDB.Models
{
    [Table("Accessories")]
    public class Accessory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Accessories { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        // Навигационные свойства
        public virtual ICollection<VehiclesAccessories> VehiclesAccessories { get; set; } = new List<VehiclesAccessories>();

        // Метод для проверки допустимых значений
        public static bool IsValidName(string name)
        {
            var validNames = new List<string>
            {
                "Детское кресло",
                "Кабриолет",
                "Кондиционер",
                "багаж на крыше"
            };
            return validNames.Contains(name);
        }
    }
}