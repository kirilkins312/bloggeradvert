using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Models
{
    public class BloggerUser : User
    {


        [Required]
        [StringLength(200)]
        public string Name { get; set; }
     

        public string InstagramAvatar { get; set; }//

        public int InstagramFollowers { get; set; }//

        public string Biography { get; set; }//

        [StringLength(50)]
        public string InstagramUsername { get; set; }//
        //public string Address { get; set; }

        // Связь многие ко многим с категориями



        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }


        public string CoverImageUrl { get; set; }

        /// <summary>
        /// //STRIPE BUSSINESSPROFILE DATA
        /// </summary>
        public string? BussinessName { get; set; }
        public string? ProductDescription { get; set; }
        public string? Url { get; set; }
        public string? SupportPhone { get; set; }

        /// <summary>
        /// //STRIPE INDIVIDUAL DATA
        /// </summary>
        /// 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        /// <summary>
        /// /DOB data
        /// </summary>
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        /// <summary>
        /// / CONTACT DATA
        /// </summary>
        //Default
        /// <summary>
        /// / ADDRESS
        /// </summary>
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        /// <summary>
        /// USER STRIPE ACCOUNT ID
        /// </summary>
        public string? UserStripeId {  get; set; }
        public bool StripeAccCreated { get; set; } = false;
        public bool StripeAccActivated { get; set; } = false;
        public bool StripeReqDataFilled { get; set; } = false;

        // Конструктор для инициализации коллекций
        public BloggerUser()
        {
            Categories = new HashSet<Category>();
            Transactions = new HashSet<Transaction>();
        }

    }
}
