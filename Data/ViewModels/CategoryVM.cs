using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Instadvert.CZ.Models;

namespace Instadvert.CZ.Data.ViewModels
{
    public class CategoryVM
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

   
        public virtual ICollection<BloggerUser>? userBloggers { get; set; }
        public virtual ICollection<CompanyUser>? userCompanies { get; set; }


    }
}
