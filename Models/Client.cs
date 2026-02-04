using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace CarSharingDB.Models
{
    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Client { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Отчество")]
        public string? Father_name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime Date_of_birth { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        [Display(Name = "Номер телефона")]
        public string Phone_number { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Водительское удостоверение")]
        public string License { get; set; }

        // Навигационные свойства
        public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();
        public virtual ICollection<ClientPayments> ClientPayments { get; set; } = new List<ClientPayments>();
    }
}