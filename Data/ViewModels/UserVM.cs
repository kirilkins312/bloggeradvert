using Instadvert.CZ.Data.Static;
using Instadvert.CZ.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Data.ViewModels
{
    public class UserVM
    {

        public string? Id { get; set; }
        public string? StripeUserId { get; set; }
        //GENERAL PROPERTIES////////
        public string Password {  get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name ="Username")]
        public string UserName { get; set; }
        public string SelectedRole { get; set; }

        /// <summary>
        /// COLLECTIONS AND LISTS
        /// </summary>

        public ICollection<Category>? CategoryList { get; set; }
        public List<int> SelectedCategories { get; set; }
        public List<string> userRoles { get; set; }
        public string Role { get; set; }
        public List<BloggerUser> blogUsers { get; set; }
        public List<CompanyUser> compUsers { get; set; }

        //COMPANY PROPERTIES//////////


        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string PhoneNumberPrefix { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        //BLOGGER PROPERTIES///////////////////
        /// <summary>
        /// //STRIPE BUSSINESSPROFILE DATA
        /// </summary>
        /// 

        [Display(Name = "Bussiness name")]
        public string? BussinessName { get; set; }
        [Display(Name = "Product description")]
        public string? ProductDescription { get; set; }

        public string? Url { get; set; }
        [Display(Name = "Support phone")]
        public string? SupportPhone { get; set; }

        /// <summary>
        /// //STRIPE INDIVIDUAL DATA
        /// </summary>
        /// 

        [Display(Name="First name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last name")]
        public string? LastName { get; set; }
        /// <summary>
        /// /DOB data
        /// </summary>
        public int? Day { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }

        public int? Age { get; set; }
        /// <summary>
        /// / CONTACT DATA
        /// </summary>
        //Default
        /// <summary>
        /// / ADDRESS
        /// </summary>
        /// 
        [Display(Name="Line 1")]
        public string? Line1 { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }







        /// <summary>
        /// ////////////////////////INSTAGRAM DATA
        /// </summary>
        /// 
        /// 

        [Display(Name = "Instagram avatar")]
        public string InstagramAvatar { get; set; }
        [Display(Name = "Followers")]
        public int InstagramFollowers { get; set; }

        public string Biography { get; set; }//
        [StringLength(50)]
        [Display(Name = "Instagram username")]
        public string InstagramUsername { get; set; }
        public IFormFile CoverPhoto { get; set; }
        public string CoverImageUrl { get; set; }

        //////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        public virtual bool TwoFactorCustomEnabled { get; set; }
        public virtual bool AccountDeactivated { get; set; } 
        public DateTime? DeactivatedAt { get; set; }
        public DateTime? EmailChanged { get; set; }
        public DateTime? PhonenumberChanged { get; set; }
        ////////////////////////////////////////////////////
        public bool StripeAccCreated { get; set; } 
        public bool StripeAccActivated { get; set; } 
        public bool StripeReqDataFilled { get; set; } 



    }
}
