using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharingDB.Models
{
    [Table("Client_Payments")]
    public class ClientPayments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Client_Payments { get; set; }

        // Внешние ключи
        [Required]
        [ForeignKey("Client")]
        public int ID_Client { get; set; }

        [Required]
        [ForeignKey("Payment")]
        public int ID_Payments { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Тип платежа")]
        public string Type_of_payment { get; set; }

        // Навигационные свойства
        [Required]
        public virtual Client Client { get; set; }

        [Required]
        public virtual Payment Payment { get; set; }

        // Метод для проверки допустимых типов платежей
        public static bool IsValidPaymentType(string type)
        {
            var validTypes = new List<string> { "Штраф", "Плата за аренду", "Ремонт" };
            return validTypes.Contains(type);
        }
    }
}