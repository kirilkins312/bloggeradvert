using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Models
{
    public class BloggerUser : User
    {
        // Required field for the name of the blogger, with a max length of 200 characters.
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        // Optional fields for storing the blogger's Instagram data.
        public string InstagramAvatar { get; set; } // URL or path to the Instagram avatar.
        public int InstagramFollowers { get; set; } // Number of Instagram followers.
        public string Biography { get; set; } // Blogger's biography.

        // Instagram username with a maximum length of 50 characters.
        [StringLength(50)]
        public string InstagramUsername { get; set; }

        // Many-to-many relationship between BloggerUser and Category entities.
        public virtual ICollection<Category> Categories { get; set; }

        // Relationship indicating that BloggerUser can have multiple transactions.
        public virtual ICollection<Transaction> Transactions { get; set; }

        // Optional field for storing a URL of the blogger's cover image.
        public string CoverImageUrl { get; set; }

        /// <summary>
        /// STRIPE BUSINESS PROFILE DATA
        /// These fields are required for integrating Stripe's business profile data.
        /// </summary>
        public string? BussinessName { get; set; } // The name of the blogger's business.
        public string? ProductDescription { get; set; } // Description of the product the blogger offers.
        public string? Url { get; set; } // URL for the blogger's business page.
        public string? SupportPhone { get; set; } // Phone number for support.

        /// <summary>
        /// STRIPE INDIVIDUAL DATA
        /// Personal information fields required for Stripe individual account setup.
        /// </summary>
        public string? FirstName { get; set; } // First name of the individual.
        public string? LastName { get; set; } // Last name of the individual.

        /// <summary>
        /// DOB (Date of Birth) data required by Stripe for verification.
        /// </summary>
        public int? Day { get; set; } // Day of birth.
        public int? Month { get; set; } // Month of birth.
        public int? Year { get; set; } // Year of birth.

        /// <summary>
        /// Contact details such as address.
        /// </summary>
        public string? Line1 { get; set; } // Street address (line 1).
        public string? City { get; set; } // City name.
        public string? Country { get; set; } // Country code.
        public string? PostalCode { get; set; } // Postal code.

        /// <summary>
        /// Stripe account-related fields to track user onboarding status and account information.
        /// </summary>
        public string? UserStripeId { get; set; } // Stripe account ID for the user.
        public bool StripeAccCreated { get; set; } = false; // Whether the Stripe account has been created.
        public bool StripeAccActivated { get; set; } = false; // Whether the Stripe account has been activated.
        public bool StripeReqDataFilled { get; set; } = false; // Whether all required Stripe data has been filled.

        // Constructor to initialize the collections (Categories and Transactions).
        public BloggerUser()
        {
            Categories = new HashSet<Category>();
            Transactions = new HashSet<Transaction>();
        }
    }

}
