using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Instadvert.CZ.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public string TransactionBloggerUserId { get; set; }

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

        // Навигационные свойства
        [ForeignKey("TransactionBloggerUserId")]
        public virtual User? Blogger { get; set; }
        [ForeignKey("TransactionCompanyUserId")]
        public virtual User? Company { get; set; }
    }
}
