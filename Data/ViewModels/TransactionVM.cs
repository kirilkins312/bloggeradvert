using Instadvert.CZ.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Instadvert.CZ.Data.ViewModels
{
    public class TransactionVM
    {

       
        public int TransactionId { get; set; }


        public string? TransactionBloggerUserId { get; set; }

        public string? TransactionBloggerUserStripeId { get; set; }
        public string? TransactionCompanyUserId { get; set; }

        [Required]
       
        public int Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Status { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        public virtual List<Transaction>? AllTransactions { get; set; }
        public virtual List<Transaction>? Transactions { get; set; }

        // Навигационные свойства
        [ForeignKey("TransactionBloggerUserId")]
        public virtual BloggerUser Blogger { get; set; }
        [ForeignKey("TransactionCompanyUserId")]
        public virtual CompanyUser Company { get; set; }
    }
}
