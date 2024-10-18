using Humanizer;
using Instadvert.CZ.Data;
using Instadvert.CZ.Data.Static;
using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Stripe;
using Stripe.Checkout;
using Stripe.Identity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Instadvert.CZ.Controllers
{
    public class StripeController : Controller
    {
        
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public StripeController(IConfiguration configuration, Microsoft.AspNetCore.Identity.UserManager<User> userManager, ApplicationDbContext context)
        {
           
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        //Page where users can provide the information required to create a Stripe profile.
        public async Task<IActionResult> StripeActivationPage(string Id)
        {

            var user = await _userManager.FindByIdAsync(Id);

            ViewBag.Id = user.Id;


            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            if (Id != user.Id)
            {
                return NotFound();
            }
            else
            {

                var blogUser = user as BloggerUser;

                ViewBag.StripeId = blogUser.UserStripeId;

                var editBlogVM = new UserVM()
                {
                    Id = blogUser.Id,
                    BussinessName = blogUser.BussinessName,
                    ProductDescription = blogUser.ProductDescription,
                    Url = blogUser.Url,
                    SupportPhone = blogUser.PhoneNumberPrefix + user.PhoneNumber,
                    FirstName = blogUser.FirstName,
                    LastName = blogUser.LastName,
                    Day = blogUser.Day,
                    Month = blogUser.Month,
                    Year = blogUser.Year,
                    Line1 = blogUser.Line1,
                    City = blogUser.City,
                    Country = blogUser.Country,
                    PostalCode = blogUser.PostalCode,
                   
                    StripeAccCreated = blogUser.StripeAccCreated,
                    StripeAccActivated = blogUser.StripeAccActivated,



                };

                return View(editBlogVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> StripeActivationPage(string Id, UserVM model)
        {
            var user = await _userManager.FindByIdAsync(Id);
            ViewBag.Id = model.Id;
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            if (Id != user.Id)
            {
                return NotFound();
            }

            var blogUser = user as BloggerUser;

            if (blogUser == null)
            {
                ViewBag.Message = "Something went wrong";
                return View("Error");
            }


            ViewBag.StripeId = blogUser.UserStripeId;
            blogUser.BussinessName = model.BussinessName;
            blogUser.ProductDescription = model.ProductDescription;
            blogUser.Url = model.Url;
            blogUser.SupportPhone = model.SupportPhone;
            blogUser.FirstName = model.FirstName;
            blogUser.LastName = model.LastName;
            blogUser.Day = model.Day;
            blogUser.Month = model.Month;
            blogUser.Year = model.Year;
            blogUser.Line1 = model.Line1;
            blogUser.City = model.City;
            blogUser.Country = model.Country;
            blogUser.PostalCode = model.PostalCode;
            




            try
            {
                _context.Update(blogUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                ViewBag.Message = "Something went wrong";
                return View("Error");


            }
            ViewBag.Message = "Succesfully edited";
            return View("Message");





        }


        //Send money to blogger 
        public IActionResult CreateTransfer(int transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(x => x.TransactionId == transactionId);




            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";
            var options = new TransferCreateOptions
            {
                Amount = transaction.Amount*100,
                Currency = "czk",
                Destination = transaction.TransactionBloggerUserStripeId,

            };
            var service = new TransferService();
            service.Create(options);

            transaction.Status = Status.Paid;
            _context.Update(transaction);
            _context.SaveChanges();

            return View();
        }

        //List of all transactions
        public async Task<IActionResult> TransactionsList(string id)
        {
            if (id == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            var transactionVM = new TransactionVM();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            var transactions = _context.Transactions
                .Include(x => x.TransactionBloggerUserId) // Include the Blogger entity
                .Include(x => x.TransactionCompanyUserId)
                .Where(x =>x.TransactionCompanyUserId == id || x.TransactionBloggerUserId == id )// Include the Company entity               
                .ToList();

            var allTransactions = _context.Transactions
                .Include(x => x.TransactionBloggerUserId) // Include the Blogger entity
                .Include(x => x.TransactionCompanyUserId)
                .ToList();

            transactionVM.Transactions = transactions;
            transactionVM.AllTransactions = allTransactions; // Assuming AllTransactions should also be the filtered list

            return View(transactionVM);
        }


        //Create Stripe account for user. 
        public async Task<IActionResult> CreateUser(string id)
        {
            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp"; //right

            if(id == null)
            {
                ViewBag.Message = "User does not exist";
                ViewBag.SecondaryMessage = "";
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(id) as BloggerUser;

            if(user.StripeAccCreated == true)
            {
                ViewBag.Message = "You already have created Stripe account";
                ViewBag.SecondaryMessage = "";
                return View("Message");
            }

            // Check if string properties are null or empty
            bool isValid = !string.IsNullOrEmpty(user.BussinessName) &&
                           !string.IsNullOrEmpty(user.ProductDescription) &&
                           !string.IsNullOrEmpty(user.Url) &&
                           
                           !string.IsNullOrEmpty(user.FirstName) &&
                           !string.IsNullOrEmpty(user.LastName) &&
                           !string.IsNullOrEmpty(user.Line1) &&
                           !string.IsNullOrEmpty(user.City) &&
                           !string.IsNullOrEmpty(user.Country) &&
                           !string.IsNullOrEmpty(user.PostalCode);
                           

            // Check if Date properties are valid (e.g., you might want to validate they are within a reasonable range)
            bool isDateValid = user.Day > 0 && user.Day <= 31 &&
                               user.Month > 0 && user.Month <= 12 &&
                               user.Year > 1900; // Example: valid year should be more than 1900

            if (isDateValid == false || isValid == false)
            {
                ViewBag.Message = "You are missing or failing required data";
                ViewBag.SecondaryMessage = "Fill and check all required data, make sure, that there aren't empty spaces and nulls. ";
                return View("Error");
            }
            // Concatenate phone number prefix and the actual phone number.
            var phonenum = user.PhoneNumberPrefix + user.PhoneNumber;

            try
            {
                // Create a new Stripe account for an individual user.
                // This structure matches the Stripe API's account creation requirements.
                var accountOptions = new AccountCreateOptions
                {
                    BusinessProfile = new AccountBusinessProfileOptions
                    {
                        Name = user.BussinessName,
                        ProductDescription = user.ProductDescription,
                        Url = user.Url,
                        SupportPhone = phonenum,
                    },
                    Individual = new AccountIndividualOptions
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Dob = new DobOptions
                        {
                            Day = user.Day,
                            Month = user.Month,
                            Year = user.Year,
                        },
                        Email = user.Email,
                        Phone = phonenum,
                        Address = new AddressOptions
                        {
                            Line1 = user.Line1,
                            City = user.City,
                            Country = user.Country,
                            PostalCode = user.PostalCode
                        }
                    },
                    BusinessType = "individual",
                    Type = "standard",
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
                    },
                };

                var service = new AccountService();
                var account = await service.CreateAsync(accountOptions);
                // Save the new Stripe account ID to the user and mark that the account is created.
                user.UserStripeId = account.Id;
                user.StripeAccCreated = true;
                _context.Update(user);
                await _context.SaveChangesAsync();

                return View(account);
            }
          
            catch (Exception ex)
            {
                // Handle other potential exceptions
                ViewBag.Message = "An unexpected error occurred, enter valid data and try again";
                ViewBag.SecondaryMessage = ex.Message;
                return View("Error");
            }
        }

        //Form where user can create request for money transfer and send it to specified company
        [HttpGet]
        public async Task<IActionResult> TransactionForm(string companyId, string bloggerId)
        {
            var user = await _userManager.FindByIdAsync(bloggerId) as BloggerUser;
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }
            if (user.UserStripeId == null)
            {
                ViewBag.Message = "You have not activated your Stripe account yet.";
                return View("Error");
            }

            var vm = new TransactionVM()
            {
                TransactionCompanyUserId = companyId,
                TransactionBloggerUserId = bloggerId,
                TransactionBloggerUserStripeId = user.UserStripeId,
                Status = Status.Created,
                
            };
            return View(vm);

        }

        //Form where user can create request for money transfer and send it to specified company

        [HttpPost]
        public async Task<IActionResult> TransactionForm(TransactionVM model)
        {
            if(model.Amount >= 99999999)
            {

                ViewBag.Message = "Amount have to be less then 99999999";
                return View("Error");
            }

            var transaction = new Transaction
            {
                TransactionCompanyUserId = model.TransactionCompanyUserId,
                TransactionBloggerUserId = model.TransactionBloggerUserId,
                TransactionBloggerUserStripeId = model.TransactionBloggerUserStripeId,
                Status = model.Status,
                Timestamp = DateTime.UtcNow,
                Amount = model.Amount,
                Description = model.Description,
                
            };

            _context.Add(transaction);
            _context.SaveChanges();


            ViewBag.Message = "Thanks for your patience";
            ViewBag.SecondaryMessage = "We will let you know about next steps";
            return View("Message");

        }

        // This method is responsible for creating a Stripe Checkout session and redirecting the user(Company) to Stripe's payment page.
        public async Task<IActionResult> StripeCheckout(int? transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(x => x.TransactionId == transactionId);

            var amount = transaction.Amount * 100;

            var priceOptions = new PriceCreateOptions
            {
                Currency = "czk",
                
                UnitAmount = transaction.Amount*100,
                ProductData = new PriceProductDataOptions { Name = "Transaction" },
            };
            var priceService = new PriceService();
            var price = priceService.Create(priceOptions);

            

            var options = new Stripe.Checkout.SessionCreateOptions
            {

                SuccessUrl = $"https://localhost:7211/Stripe/StripeSuccess?transactionId={HttpUtility.UrlEncode(transaction.TransactionId.ToString())}",

                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
    {
        new Stripe.Checkout.SessionLineItemOptions
        {
            Price = price.Id,
            Quantity = 1,
        },
    },
                Mode = "payment",
            };
            var service = new Stripe.Checkout.SessionService();
            Session session = service.Create(options);
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

        }

        // Updating status for transaction. Status "Success" is reached after successful money transfer from company on our Stripe account. After that admin can send money to blogger.
        public async Task<IActionResult> StripeSuccess(int transactionId)
        {
            var transaction =  _context.Transactions.FirstOrDefault(x=>x.TransactionId == transactionId);   
            transaction.Status = "Success";
            transaction.Timestamp = DateTime.Now;
            _context.Update(transaction);
            _context.SaveChanges();
            var vm = new TransactionVM { Status = transaction.Status};
            return View(vm);

        }


        // Onboard account to our platform. A money transaction cannot be processed without completing onboarding. This action connecting blogger to our Stripe platform. 
        // Action is redirecting user to Sripe page, where user accepting Terms of use etc. and confirming data that he provided earlier.
        // After accepting account is onboarded and ready.
        public async Task<IActionResult> ActivateAccount(string? id)
        {
            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp"; //right

            var user = await _userManager.FindByIdAsync(id) as BloggerUser;

            if (user == null)
            { 
                ViewBag.Message = "User does not exist";
                ViewBag.SecondaryMessage = "Try again later";
                return View("Error");

            }

            // Create a new account link to onboard the new account.
            var options = new AccountLinkCreateOptions
            {
                Account = user.UserStripeId,
                RefreshUrl = "http://localhost:4242/onboard-user/refresh",
                ReturnUrl = $"https://localhost:7211/Stripe/StripeActivationSuccess?id={HttpUtility.UrlEncode(user.Id.ToString())}",
                Type = "account_onboarding",
            };
            var service = new AccountLinkService();
            var accountLink = service.Create(options);
            return Redirect(accountLink.Url);

        }

        // Changing account status to activated
        public async Task<IActionResult> StripeActivationSuccess(string? id)
        {
            var user = await _userManager.FindByIdAsync(id) as BloggerUser;
            if (user == null) {
                ViewBag.Message = "Something went wrong";
                ViewBag.SecondaryMessage = "Try again later";
                return View("Error");
            }
            user.StripeAccActivated = true;
            _context.Update(user);
            _context.SaveChanges();
            return View(user);
        }
    }

}
