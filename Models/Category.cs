using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace Instadvert.CZ.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public virtual ICollection<CompanyUser> companyUsers { get; set; }
        public virtual ICollection<BloggerUser> bloggerUserUsers { get; set; }    
        // Конструктор для инициализации коллекций
        public Category()
        {
            bloggerUserUsers = new HashSet<BloggerUser>();
            companyUsers = new HashSet<CompanyUser>();
        }   
    }
}
