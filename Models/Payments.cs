using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSharingDB.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Payments { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Способ оплаты")]
        public string Way_of_payment { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        [Display(Name = "Сумма")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Amount { get; set; }

        // Навигационные свойства
        public virtual ICollection<ClientPayments> ClientPayments { get; set; } = new List<ClientPayments>();

        // Метод для проверки допустимых способов оплаты
        public static bool IsValidPaymentMethod(string method)
        {
            var validMethods = new List<string> { "Карта", "Наличные", "QR-код" };
            return validMethods.Contains(method);
        }
    }
}