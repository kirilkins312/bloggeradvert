using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Models
{
    public class CompanyUser : User
    {
        

        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }


        //Конструктор для инициализации коллекций
        public CompanyUser()
        {
            Categories = new HashSet<Category>();
            Transactions = new HashSet<Transaction>();
        }
    }
}
