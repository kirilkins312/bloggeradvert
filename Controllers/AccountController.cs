using Instadvert.CZ.Data;
using Instadvert.CZ.Data.Static;
using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Data.Entity;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Instadvert.CZ.Data.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using static QRCoder.PayloadGenerator;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Ajax.Utilities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNet.Identity;
using System.Composition;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.BuilderProperties;
using System.Diagnostics.Metrics;
using Twilio.Rest.Api.V2010.Account.Usage.Record;


namespace Instadvert.CZ.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ISenderEmail _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Random _random;
        

        public AccountController(Microsoft.AspNetCore.Identity.UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context, ISenderEmail emailSender, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _random = new Random();
            _webHostEnvironment = webHostEnvironment;


        }

        /// <summary>
        /// ///////////////
        /// </summary>
        /// EMAIL HANDLING
        /// <returns></returns>





        [HttpGet]
        public async Task<IActionResult> ChangeEmail(string Id)
        {
            // Find the user by their Id using the UserManager service
            var user = await _userManager.FindByIdAsync(Id);

            // Prepare a list of possible roles for the user
            var roles = new List<string>
    {
       UserRoles.Blogger,   // Blogger role
       UserRoles.Company    // Company role
    };

            // Store the roles and the user's Id in ViewBag to pass them to the View
            ViewBag.Roles = roles;
            ViewBag.Id = user?.Id;

            // Check if the user object is null (user not found)
            if (user == null)
            {
                // If the user doesn't exist, display an error message
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            // Validate if the passed Id matches the user's Id (as an additional security check)
            if (Id != user.Id)
            {
                return NotFound();
            }

            // Check if the user is in the "Blogger" role
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                // Cast the user to the BloggerUser type
                var blogUser = user as BloggerUser;

                // Create a ViewModel for editing a Blogger's email
                var editBlogVM = new UserVM()
                {
                    Email = blogUser?.Email,   // Set the email from the BloggerUser
                    userRoles = roles,         // Pass available roles
                    SelectedRole = "Blogger"   // Set the selected role to Blogger
                };

                // Return the view with the Blogger-specific ViewModel
                return View(editBlogVM);
            }

            // Check if the user is in the "Company" role
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                // Cast the user to the CompanyUser type
                var compUser = user as CompanyUser;

                // Create a ViewModel for editing a Company's email
                var editCompVM = new UserVM()
                {
                    Email = compUser?.Email,   // Set the email from the CompanyUser
                    userRoles = roles,         // Pass available roles
                    SelectedRole = "Company"   // Set the selected role to Company
                };

                // Return the view with the Company-specific ViewModel
                return View(editCompVM);
            }

            // If the user is not found in any of the specified roles, return NotFound
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(string id, UserVM model)
        {
            // Find the user by their Id using the UserManager service
            var user = await _userManager.FindByIdAsync(id);

            // Check if the user object is null (user not found)
            if (user == null)
            {
                ViewBag.Message = "User does not exist";  // Inform that the user doesn't exist
                return View("Error");  // Return an error view
            }

            // Ensure the Id passed in the request matches the user's actual Id
            if (id != user.Id)
            {
                return NotFound();  // Return a 404 Not Found if the Ids don't match
            }

            // Check if the user is in the "Blogger" role
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                // Find the user again and cast them to BloggerUser type
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;

                // Update the BloggerUser's email with the new email from the form
                blogUser.Email = model.Email;

                try
                {
                    // Try to update the BloggerUser entity in the database
                    _context.Update(blogUser);
                    await _context.SaveChangesAsync();  // Save changes asynchronously
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Catch any concurrency issues that may occur during the update
                    ViewBag.Message = "Something went wrong";  // Inform the user of an error
                    return View("Error");  // Return an error view
                }

                // If successful, inform the user that the email was edited successfully
                ViewBag.Message = "Successfully edited";
                return View("Message");  // Return a success message view
            }

            // Check if the user is in the "Company" role
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                // Find the user again and cast them to CompanyUser type
                var companyUser = await _userManager.FindByIdAsync(id) as CompanyUser;

                // Update the CompanyUser's email with the new email from the form
                companyUser.Email = model.Email;

                try
                {
                    // Try to update the CompanyUser entity in the database
                    _context.Update(companyUser);
                    await _context.SaveChangesAsync();  // Save changes asynchronously
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Catch any concurrency issues that may occur during the update
                    ViewBag.Message = "Something went wrong";  // Inform the user of an error
                    return View("Error");  // Return an error view
                }

                // If successful, inform the user that the email was edited successfully
                ViewBag.Message = "Successfully edited";
                return View("Message");  // Return a success message view
            }

            // If the user is not in either role, return an error message
            ViewBag.Message = "Something went wrong";
            return View("Error");
        }



        private async Task SendConfirmationEmail(string? email, User? user)
        {
            if(user == null)
            {
                ViewBag.Message = "Something went wrong";
                RedirectToAction("Error");
            }
            var expiry = DateTime.UtcNow.AddMinutes(5);
            //Generate the Token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //Build the Email Confirmation Link which must include the Callback URL
            var ConfirmationLink = Url.Action("ConfirmEmail", "Account",
            new { UserId = user.Id, Token = token, Expired = expiry.ToString("o") }, protocol: HttpContext.Request.Scheme);
            //Send the Confirmation Email to the User Email Id
            await _emailSender.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(ConfirmationLink)}'>clicking here</a>.", true);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmationEmail(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null || await _userManager.IsEmailConfirmedAsync(user))
            {
                // Handle the situation when the user does not exist or Email already confirmed.
                // For security, don't reveal that the user does not exist or Email is already confirmed
                ViewBag.Message = "User does not exist, or you have already confirmed your e-mail";
                return View("Message");
            }
            //Then send the Confirmation Email to the User
            await SendConfirmationEmail(Email, user);
            ViewBag.Message = "Confirmation Email Sent. If Any Account Exists with the Given Email Id, then Email Confirmation Link is Sent.";
            return View("Message");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResendConfirmationEmail(bool IsResend = true)
        {
            if (IsResend)
            {
                ViewBag.Message = "Resend Confirmation Email";
            }
            else
            {
                ViewBag.Message = "Send Confirmation Email";
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string UserId, string Token, string Expiry)
        {
            // Try to parse the Expiry string into a DateTime object
            if (DateTime.TryParse(Expiry, out DateTime expiryDate))
            {
                // Check if the link has expired by comparing expiry date to current UTC time
                if (expiryDate < DateTime.UtcNow)
                {
                    ViewBag.Message = "This confirmation link has expired.";  // Inform the user about expiration
                    return View("Error");  // Return an error view
                }
            }

            // Define a specific date (could be for comparison purposes)
            DateTime newDate = new DateTime(2024, 5, 4);

            // Check if the UserId or Token is null or if the expiry date is earlier than the predefined date
            if (UserId == null || Token == null || expiryDate < newDate)
            {
                ViewBag.Message = "The link is Invalid or Expired";  // Inform the user of an invalid or expired link
                return View("Error");  // Return an error view
            }

            // Attempt to find the user by their UserId
            var user = await _userManager.FindByIdAsync(UserId);

            // If the user does not exist, return an error view
            if (user == null)
            {
                ViewBag.Message = "User does not exist";  // Inform that the user wasn't found
                return View("Error");  // Return an error view
            }

            // Check if the user's email is already confirmed
            if (user.EmailConfirmed)
            {
                ViewBag.Message = "Email is already confirmed";  // Inform that the email was already confirmed
                return View("Error");  // Return an error view
            }

            // If the user exists and the email isn't confirmed yet, confirm the email with the provided token
            var result = await _userManager.ConfirmEmailAsync(user, Token);

            // Check if the email confirmation succeeded
            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email";  // Success message after confirming the email
                return View();  // Return a success view
            }

            // If the confirmation failed for some reason, return an error view
            ViewBag.Message = "Email cannot be confirmed";  // Inform that the email could not be confirmed
            return View("Error");  // Return an error view
        }
        //2STEPVERIFICATION/////////////////////////////////////////////////////////////////////////////////////////////////////////



        private async Task<VerifyVM> SendVerificationCodeByEmail(string email)
        {
            string verificationCode = _random.Next(100000, 999999).ToString();
            //inserting code and expire data for next steps
            var model = new VerifyVM()
            {
                Code = verificationCode,
                Expiry = DateTime.UtcNow.AddMinutes(5),
            };
            string message = $"Your verification code is: {verificationCode}";
            await _emailSender.SendEmaiVerificationlAsync(email, "Verification Code", message);
            return model;
        }

        [HttpGet]
        public IActionResult VerifyVerificationCodeGet(VerifyVM model)
        {
            //receiving user and verification data 
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> VerifyVerificationCode(VerifyVM model)
        {
            //Checking if data aint expired and received code from user and 
            //generated code are same
            if (model.Expiry > DateTime.UtcNow && model.Code == model.UserCode)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                model.Id = user.Id;
                // Cheking password (again)
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {

                    return View("SuccesfullLogin");
                }

                return View("Error",ViewBag.Message = "Something went wrong, try again later");



            }
            ViewBag.Message = "Invalid verification code, try again";
            return View("VerifyVerificationCodeGet");
        }


        /// <summary>
        /// //////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        ////LOGIN AND REGISTER////////////////////////////////////////////////
        public IActionResult Login()
        {   
            var loginVM =  new LoginVM();
            return View(loginVM);
        }
       
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            // Look up the user by their email
            var user = await _userManager.FindByEmailAsync(loginVM.Email);

            // Check if user exists
            if (user != null)
            {
                // Check if the user's account is deactivated
                if (user.AccountDeactivated == true)
                {
                    ViewBag.Message = "User does not exist";  // Handle deactivated accounts
                    return View("Error");
                }

                // Verify the user's password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    // Check if the user's email is confirmed
                    if (user.EmailConfirmed == true)
                    {
                        // Check if the user has Two-Factor Authentication (2FA) enabled
                        if (user.TwoFactorCustomEnabled == true)
                        {
                            string email = user.Email;

                            // Send a verification code via email for 2FA
                            var model = await SendVerificationCodeByEmail(email);

                            // Attach additional data (email, password) to the model used for 2FA verification
                            model.Email = email;
                            model.Password = loginVM.Password;

                            // Redirect to the 2FA verification view
                            return View("VerifyVerificationCodeGet", model);
                        }
                        else
                        {
                            // If 2FA is not enabled, proceed with password sign-in
                            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                            if (result.Succeeded)
                            {
                                // Login successful
                                ViewBag.Message = "Congratulations";
                                ViewBag.SecondaryMessage = "You are now logged in";
                                return View("Message");
                            }

                            // Handle case where login fails despite correct password
                            return View("Error", ViewBag.Message = "Something went wrong, try again later");
                        }
                    }

                    // Redirect to registration success view if email isn't confirmed
                    return View("RegistrationSuccessful");
                }

                // Handle wrong password scenario
                ViewBag.Message = "Wrong credentials, try again";
                return View(loginVM);
            }

            // Handle scenario where user does not exist or credentials are incorrect
            ViewBag.Message = "Wrong credentials, try again";
            return View(loginVM);
        }
        public IActionResult Registration()
        {
            // Prepare a list of roles available for the user to choose from during registration
            var roles = new List<string>
    {
       UserRoles.Blogger,   // Blogger role
       UserRoles.Company    // Company role
    };

            ViewBag.Roles = roles;  // Pass the roles to the view using ViewBag

            // Retrieve the list of categories from the database
            var categories = _context.Categories.ToList();

            // Initialize the ViewModel that will be passed to the view
            var model = new Data.ViewModels.UserVM();

            // Check if the category list contains any items
            if (categories.Count > 0)
            {
                // Assign the retrieved categories and roles to the ViewModel
                model.CategoryList = categories;  // Set the category list in the ViewModel
                model.userRoles = roles;  // Set the available roles in the ViewModel
            }
            else
            {
                // If no categories were retrieved, display an error message and return an error view
                return View("Error", ViewBag.Message = "Something went wrong, try again later");
            }

            // Store the category list in ViewBag for easy access in the view
            ViewBag.Categories = categories;

            // Return the registration view, passing the populated ViewModel
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Registration(UserVM signInVM)
        {
            // Retrieve list of roles from the database and pass them to the view
            var roles = _context.UserRoles.ToList();
            ViewBag.Roles = roles;

            // Check if the email provided during registration is already in use
            var user = await _userManager.FindByEmailAsync(signInVM.Email);
            if (user != null)
            {
                // If email is already taken, display an error and redirect back to the registration page
                TempData["Error"] = "This email is already taken";
                return RedirectToAction("Registration");
            }

            // Handle registration logic for the "Blogger" role
            if (signInVM.SelectedRole == "Blogger")
            {
                // If a cover photo is provided, upload the image and save its URL
                if (signInVM.CoverPhoto != null)
                {
                    string folder = "Images/cover/";  // Define the folder where the image will be stored
                    signInVM.CoverImageUrl = await UploadImage(folder, signInVM.CoverPhoto);  // Upload the image
                }

                // Create a new BloggerUser object and assign the registration data to it
                var newUser = new BloggerUser()
                {
                    Email = signInVM.Email,
                    InstagramUsername = signInVM.InstagramUsername,
                    InstagramAvatar = signInVM.InstagramAvatar,
                    InstagramFollowers = signInVM.InstagramFollowers,
                    UserName = signInVM.UserName,
                    Name = signInVM.UserName,
                    PhoneNumber = signInVM.PhoneNumber,
                    PhoneNumberPrefix = signInVM.PhoneNumberPrefix,
                    CoverImageUrl = signInVM.CoverImageUrl,  // Set the uploaded cover image URL
                    Biography = signInVM.Biography,
                    Role = signInVM.SelectedRole  // Assign the selected role to the user
                };

                // Add the selected categories to the new BloggerUser's categories
                foreach (var categoryId in signInVM.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);  // Find each category by its ID
                    if (selectedCategory != null)
                    {
                        newUser.Categories.Add(selectedCategory);  // Add the category to the user's categories
                    }
                }

                // Attempt to create the new BloggerUser in the database
                var newUserResponse = await _userManager.CreateAsync(newUser, signInVM.Password);

                if (newUserResponse.Succeeded)
                {
                    // If the user is successfully created, send a confirmation email
                    await SendConfirmationEmail(newUser.Email, newUser);

                    // Add the user to their selected role
                    await _userManager.AddToRoleAsync(newUser, signInVM.SelectedRole);

                    // Redirect to a "RegistrationSuccessful" view
                    return View("RegistrationSuccessful");
                }
                else
                {
                    // If there are errors during creation, add them to the ModelState for validation messages
                    foreach (var error in newUserResponse.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Handle registration logic for the "Company" role
            if (signInVM.SelectedRole == "Company")
            {
                // Create a new CompanyUser object and assign the registration data to it
                var newUser = new CompanyUser()
                {
                    Name = signInVM.Name,
                    Email = signInVM.Email,
                    Description = signInVM.Description,
                    Address = signInVM.Address,
                    UserName = signInVM.UserName,
                    PhoneNumber = signInVM.PhoneNumber,
                    PhoneNumberPrefix = signInVM.PhoneNumberPrefix,
                    Url = signInVM.Url,
                    Role = signInVM.SelectedRole  // Assign the selected role to the user
                };

                // Add the selected categories to the new CompanyUser's categories
                foreach (var categoryId in signInVM.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);  // Find each category by its ID
                    if (selectedCategory != null)
                    {
                        newUser.Categories.Add(selectedCategory);  // Add the category to the user's categories
                    }
                }

                // Attempt to create the new CompanyUser in the database
                var newUserResponse = await _userManager.CreateAsync(newUser, signInVM.Password);

                if (newUserResponse.Succeeded)
                {
                    // If the user is successfully created, send a confirmation email
                    await SendConfirmationEmail(newUser.Email, newUser);

                    // Add the user to their selected role
                    await _userManager.AddToRoleAsync(newUser, signInVM.SelectedRole);

                    // Redirect to a "RegistrationSuccessful" view
                    return View("RegistrationSuccessful");
                }
            }

            // If neither role registration succeeds, redirect to the home page
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// //////////////////DISPLAYING/////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        // Action to get the list of Bloggers along with their categories
        public async Task<IActionResult> BloggerList()
        {
            // Fetch categories from the database
            var categories = _context.Categories.ToList();

            // If no categories are found, return a 404 error
            if (categories == null)
            {
                return NotFound();
            }

            // Initialize the ViewModel for passing data to the view
            var model = new FilterVM();

            // Fetch all BloggerUsers from the database
            var blogUsersWithCategories = _context.Users.OfType<BloggerUser>().ToList();

            // Load the categories for each BloggerUser
            foreach (var user in blogUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            // Populate the ViewModel with the users and categories
            model.blogUsers = blogUsersWithCategories.ToList();
            model.CategoryList = categories;

            // Return the populated view
            return View(model);
        }

        // Action to get the list of Companies along with their categories
        public async Task<IActionResult> CompanyList()
        {
            // Fetch categories from the database
            var categories = _context.Categories.ToList();

            // If no categories are found, return a 404 error
            if (categories == null)
            {
                return NotFound();
            }

            // Initialize the ViewModel for passing data to the view
            var model = new FilterVM();

            // Fetch all CompanyUsers from the database
            var compUsersWithCategories = _context.Users.OfType<CompanyUser>().ToList();

            // Load the categories for each CompanyUser
            foreach (var user in compUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            // Populate the ViewModel with the users and categories
            model.compUsers = compUsersWithCategories.ToList();
            model.CategoryList = categories;

            // Return the populated view
            return View(model);
        }

        // POST Action to handle filtered Blogger list based on user inputs
        [HttpPost]
        public async Task<IActionResult> BloggerList(FilterVM model)
        {
            // Fetch categories from the database
            var categories = _context.Categories.ToList();

            // If no categories are found, return a 404 error
            if (categories == null)
            {
                return NotFound();
            }

            // Fetch all BloggerUsers from the database
            var blogUsersWithCategories = _context.Users.OfType<BloggerUser>().ToList();

            // Load the categories for each BloggerUser
            foreach (var user in blogUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            // Apply category filter if any categories are selected
            if (model.SelectedCategories != null)
            {
                foreach (var categoryId in model.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);
                    if (selectedCategory != null)
                    {
                        blogUsersWithCategories = blogUsersWithCategories
                            .Where(b => b.Categories.Any(c => c.CategoryId == categoryId)).ToList();  // Filter based on selected categories
                    }
                }
            }

            // Apply search string filter if provided
            if (model.searchString != null)
            {
                model.searchString = model.searchString.ToLower();
                // Uncomment the next line if you want to search by full name
                // blogUsersWithCategories = blogUsersWithCategories.Where(x => x.FullName.ToLower().Contains(model.searchString)).ToList();
            }

            // Apply Instagram followers filter (upper limit)
            if (model.searchStringTop != null && model.searchStringTop != 0)
            {
                blogUsersWithCategories = blogUsersWithCategories.Where(x => x.InstagramFollowers < model.searchStringTop).ToList();
            }

            // Apply Instagram followers filter (lower limit)
            if (model.searchStringBottom != null && model.searchStringBottom != 0)
            {
                blogUsersWithCategories = blogUsersWithCategories.Where(x => x.InstagramFollowers > model.searchStringBottom).ToList();
            }

            // Update the ViewModel with filtered results and category list
            model.blogUsers = blogUsersWithCategories;
            model.CategoryList = categories;

            ViewBag.Categories = categories;

            // Return the updated view
            return View(model);
        }

        // POST Action to handle filtered Company list based on user inputs
        [HttpPost]
        public async Task<IActionResult> CompanyList(FilterVM model)
        {
            // Fetch categories from the database
            var categories = _context.Categories.ToList();

            // If no categories are found, return a 404 error
            if (categories == null)
            {
                return NotFound();
            }

            // Fetch all CompanyUsers from the database
            var compUsersWithCategories = _context.Users.OfType<CompanyUser>().ToList();

            // Load the categories for each CompanyUser
            foreach (var user in compUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            // Apply category filter if any categories are selected
            if (model.SelectedCategories != null)
            {
                foreach (var categoryId in model.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);
                    if (selectedCategory != null)
                    {
                        compUsersWithCategories = compUsersWithCategories
                            .Where(b => b.Categories.Any(c => c.CategoryId == categoryId)).ToList();  // Filter based on selected categories
                    }
                }
            }

            // Apply search string filter if provided (e.g., company name)
            if (model.searchString != null)
            {
                model.searchString = model.searchString.ToLower();
                compUsersWithCategories = compUsersWithCategories
                    .Where(x => x.Name.ToLower().Contains(model.searchString)).ToList();
            }

            // Update the ViewModel with filtered results and category list
            model.compUsers = compUsersWithCategories;
            model.CategoryList = categories;

            ViewBag.Categories = categories;

            // Return the updated view
            return View(model);
        }

        /// <summary>
        /// ////////////////////EDITING & DELETING
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>


        public async Task<IActionResult> EnableTwoFactor(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        public async Task<IActionResult> EnableTwoFactorPost(string id)
        {
            // Find the user by their ID
            var user = await _userManager.FindByIdAsync(id);

            // Check if two-factor authentication (2FA) is currently enabled for the user
            if (user.TwoFactorCustomEnabled == true)
            {
                // If 2FA is enabled, disable it
                user.TwoFactorCustomEnabled = false;
                ViewBag.Message = "Two-factor authentication disabled";

                try
                {
                    // Update the user's 2FA status in the database
                    _context.Update(user);
                    await _context.SaveChangesAsync();  // Save the changes asynchronously
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If there's an issue with saving the changes, return a 404 Not Found error
                    return NotFound();
                }

                // If something went wrong, return an error view
                return View("Error");
            }

            // Check if two-factor authentication is currently disabled
            if (user.TwoFactorCustomEnabled == false)
            {
                // If 2FA is disabled, enable it
                user.TwoFactorCustomEnabled = true;

                try
                {
                    // Update the user's 2FA status in the database
                    _context.Update(user);
                    await _context.SaveChangesAsync();  // Save the changes asynchronously
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If there's an issue with saving the changes, return a 404 Not Found error
                    return NotFound();
                }

                // 2FA was successfully enabled, update the message
                ViewBag.Message = "Two-factor authentication enabled";

                // Return the error view to notify about the update (should likely be a success page)
                return View("Error");
            }

            // If no changes were made, redirect to the Home page (in case of incorrect input)
            return RedirectToAction("Home", "Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            ////////////////////////////////////////////////////////////ROLES

            // Retrieve the user by their ID from the UserManager
            var user = await _userManager.FindByIdAsync(Id);

            // Fetch all categories and add them to the ViewBag for use in the view
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.Id = user.Id; // Store the user ID in ViewBag

            // Prepare available roles (Blogger, Company) for the view
            var roles = new List<string>
    {
       UserRoles.Blogger,
       UserRoles.Company
    };
            ViewBag.Roles = roles;

            // Handle the case where the user does not exist
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            // Check if the passed ID matches the user ID
            if (Id != user.Id)
            {
                return NotFound();
            }

            // Check if the user is in the "Blogger" role
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                // Cast the user to BloggerUser and load their categories
                var blogUser = user as BloggerUser;
                await _context.Entry(blogUser).Collection(b => b.Categories).LoadAsync();

                var bloggersCategories = blogUser.Categories;

                // Mark the selected categories for the blogger in the UI
                foreach (var category in categories)
                {
                    category.IsChecked = bloggersCategories.Any(x => x.CategoryId == category.CategoryId);
                }

                // Map the BloggerUser data to the UserVM for editing
                var editBlogVM = new UserVM()
                {
                    Id = blogUser.Id,
                    UserName = blogUser.UserName,
                    CategoryList = categories,
                    Email = blogUser.Email,
                    PhoneNumber = blogUser.PhoneNumber,
                    PhoneNumberPrefix = blogUser.PhoneNumberPrefix,
                    InstagramAvatar = blogUser.InstagramAvatar,
                    InstagramFollowers = blogUser.InstagramFollowers,
                    InstagramUsername = blogUser.InstagramUsername,
                    Biography = blogUser.Biography,
                    Role = blogUser.Role,
                    SelectedRole = "Blogger"
                };

                // Return the view for editing the BloggerUser
                return View(editBlogVM);
            }

            // Check if the user is in the "Company" role
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                // Cast the user to CompanyUser and load their categories
                var compUser = user as CompanyUser;
                await _context.Entry(compUser).Collection(b => b.Categories).LoadAsync();

                var companiesCategories = compUser.Categories;

                // Mark the selected categories for the company in the UI
                foreach (var category in categories)
                {
                    category.IsChecked = companiesCategories.Any(x => x.CategoryId == category.CategoryId);
                }

                // Map the CompanyUser data to the UserVM for editing
                var editCompVM = new UserVM()
                {
                    Id = compUser.Id,
                    Name = compUser.Name,
                    Email = compUser.Email,
                    Description = compUser.Description,
                    Address = compUser.Address,
                    UserName = compUser.UserName,
                    PhoneNumber = compUser.PhoneNumber,
                    PhoneNumberPrefix = compUser.PhoneNumberPrefix,
                    Role = compUser.Role,
                    SelectedRole = "Company",
                    CategoryList = categories,
                    Url = compUser.Url
                };

                // Return the view for editing the CompanyUser
                return View(editCompVM);
            }

            // If the user is neither Blogger nor Company, return a 404 error
            return NotFound();
        }

        // POST: Handle form submission for updating the user data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserVM model)
        {
            var user = await _userManager.FindByIdAsync(id);
            ViewBag.Id = model.Id;

            // Handle the case where the user does not exist
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            // Check if the passed ID matches the user ID
            if (id != user.Id)
            {
                return NotFound();
            }

            // Update the BloggerUser details if the user is a Blogger
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;

                if (blogUser != null)
                {
                    await _context.Entry(blogUser).Collection(u => u.Categories).LoadAsync();
                }

                // Update the BloggerUser fields from the submitted form model
                blogUser.UserName = model.UserName;
                blogUser.PhoneNumberPrefix = model.PhoneNumberPrefix;
                blogUser.PhoneNumber = model.PhoneNumber;
                blogUser.InstagramAvatar = model.InstagramAvatar;
                blogUser.InstagramFollowers = model.InstagramFollowers;
                blogUser.InstagramUsername = model.InstagramUsername;
                blogUser.Biography = model.Biography;

                // Clear and update the Blogger's categories
                blogUser.Categories.Clear();
                foreach (var categoryId in model.SelectedCategories)
                {
                    var category = _context.Categories.Find(categoryId);
                    if (category != null)
                    {
                        blogUser.Categories.Add(category);
                    }
                }

                // Save changes and handle errors
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

                ViewBag.Message = "Successfully edited";
                return View("Message");
            }

            // Update the CompanyUser details if the user is a Company
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var companyUser = await _userManager.FindByIdAsync(id) as CompanyUser;

                if (companyUser != null)
                {
                    await _context.Entry(companyUser).Collection(u => u.Categories).LoadAsync();
                }

                // Update the CompanyUser fields from the submitted form model
                companyUser.UserName = model.UserName;
                companyUser.Name = model.Name;
                companyUser.PhoneNumberPrefix = model.PhoneNumberPrefix;
                companyUser.PhoneNumber = model.PhoneNumber;
                companyUser.Address = model.Address;
                companyUser.Description = model.Description;
                companyUser.Url = model.Url;

                // Clear and update the Company's categories
                companyUser.Categories.Clear();
                foreach (var categoryId in model.SelectedCategories)
                {
                    var category = _context.Categories.Find(categoryId);
                    if (category != null)
                    {
                        companyUser.Categories.Add(category);
                    }
                }

                // Save changes and handle errors
                try
                {
                    _context.Update(companyUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    ViewBag.Message = "Something went wrong";
                    return View("Error");
                }

                ViewBag.Message = "Successfully edited";
                return View("Message");
            }

            ViewBag.Message = "Something went wrong";
            return View("Error");
        }

        // Method to display the Deactivate view
        public IActionResult Deactivate(string id)
        {
            return View("Deactivate", id);
        }
        public async Task<IActionResult> DeactivateConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            // First, fetch the user you want to deactivate

            if (user == null)
            {
                // Handle the case where the user wasn't found
                ViewBag.Message = "User can not be found.";
                return View("Error");
            }
            else if (user.AccountDeactivated == true)
            {
                // Handle the case where the account is already deactivated
                ViewBag.Message = "Account is already deactivated.";
                return View("Error");
            }

            if (user != null)
            {
                // Check if the user is a Blogger
                if (await _userManager.IsInRoleAsync(user, "Blogger"))
                {
                    // Clear sensitive or irrelevant information for the Blogger user
                    var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;
                    blogUser.InstagramAvatar = "";
                    blogUser.InstagramFollowers = 0;
                    blogUser.InstagramUsername = "";
                    // Clear additional fields that are not necessary anymore
                    blogUser.EmailConfirmed = false; // Mark email as unconfirmed
                    DeleteFile(blogUser.CoverImageUrl); // Delete user's cover image file
                    blogUser.CoverImageUrl = "";
                    blogUser.Biography = "";
                    blogUser.BussinessName = "";
                    blogUser.City = "";
                    blogUser.Country = "";
                    blogUser.Day = null;
                    blogUser.FirstName = "";
                    blogUser.LastName = "";
                    blogUser.Line1 = "";
                    blogUser.Month = null;
                    blogUser.PostalCode = null;
                    blogUser.ProductDescription = "";
                    blogUser.Url = "";
                    blogUser.Year = null;

                    // Mark the account as deactivated
                    blogUser.AccountDeactivated = true;
                    blogUser.DeactivatedAt = DateTime.Now;

                    // Update the user in the database
                    _context.Update(blogUser);
                    await _context.SaveChangesAsync();

                    // Sign out the user if they are the currently logged-in user
                    if (user.Id == User.Identity.GetUserId())
                    {
                        await _signInManager.SignOutAsync();
                    }
                }
                // Check if the user is a Company
                else if (await _userManager.IsInRoleAsync(user, "Company"))
                {
                    // Clear sensitive or irrelevant information for the Company user
                    var compUser = await _userManager.FindByIdAsync(id) as CompanyUser;
                    compUser.Description = "";
                    compUser.Address = "";
                    compUser.PhoneNumber = "";
                    compUser.EmailConfirmed = false; // Mark email as unconfirmed
                    compUser.Url = "";
                    compUser.AccountDeactivated = true;
                    compUser.DeactivatedAt = DateTime.Now;

                    // Update the user in the database
                    _context.Update(compUser);
                    await _context.SaveChangesAsync();

                    // Sign out the user if they are the currently logged-in user
                    if (user.Id == User.Identity.GetUserId())
                    {
                        await _signInManager.SignOutAsync();
                    }
                }
            }

            ViewBag.Message = "Successfully deactivated";
            return View("Message");
        }

        // GET: Confirm deletion of a user account
        public IActionResult Delete(string id)
        {
            return View("Delete", id);
        }

        // POST: Confirm deletion of a user account
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                return View("Error");
            }

            // Ensure the account is deactivated before deletion
            if (user.AccountDeactivated == false)
            {
                ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                return View("Error");
            }

            if (user.DeactivatedAt == null)
            {
                ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                return View("Error");
            }

            // Remove associated transactions for the user
            var transactions = _context.Transactions.Where(x => x.TransactionCompanyUserId == id).ToList();
            foreach (var item in transactions)
            {
                item.TransactionCompanyUserId = id; // Clear the reference to the user
                item.Blogger = null; // Clear the blogger reference if necessary
            }

            // Remove messages sent by the user
            var messagesSent = _context.Messages.Where(m => m.SenderId == user.Id).ToList();
            foreach (var message in messagesSent)
            {
                _context.Messages.Remove(message);
            }

            // Remove relationships where the user is the receiver
            var messagesReceived = _context.Messages.Where(m => m.ReceiverId == user.Id).ToList();
            foreach (var message in messagesReceived)
            {
                _context.Messages.Remove(message);
            }

            // Attempt to delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                // Sign out the user if they are the currently logged-in user
                if (user.Id == User.Identity.GetUserId())
                {
                    await _signInManager.SignOutAsync();
                }

                // Handle a successful delete
                return View("DeleteConfirmed");
            }
            else
            {
                // Handle failure by adding errors to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }
        public async Task<IActionResult> Details(string id)
        {
            // Check if the user ID is null
            if (id == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            // Fetch the user by ID
            var user = await _userManager.FindByIdAsync(id);

            // Initialize the model to hold user details
            var model = new UserVM()
            {
                TwoFactorCustomEnabled = user.TwoFactorCustomEnabled,
                AccountDeactivated = user.AccountDeactivated,
                DeactivatedAt = user.DeactivatedAt,
            };

            // If user is not found, redirect to the Index page with an error message
            if (user == null)
            {
                TempData["Error"] = "User doesn't exist";
                return RedirectToAction("Index", "Home");
            }

            // Handle Blogger users
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;

                // Populate model with Blogger user details
                model.Id = blogUser.Id;
                model.SelectedRole = "Blogger";
                model.Name = blogUser.Name;
                model.InstagramAvatar = blogUser.InstagramAvatar;
                model.InstagramFollowers = blogUser.InstagramFollowers;
                model.InstagramUsername = blogUser.InstagramUsername;
                model.BussinessName = blogUser.BussinessName;
                model.ProductDescription = blogUser.ProductDescription;
                model.Url = blogUser.Url;
                model.SupportPhone = blogUser.SupportPhone;
                model.FirstName = blogUser.FirstName;
                model.LastName = blogUser.LastName;
                model.Day = blogUser.Day;
                model.Month = blogUser.Month;
                model.Year = blogUser.Year;
                model.Line1 = blogUser.Line1;
                model.City = blogUser.City;
                model.Country = blogUser.Country;
                model.PostalCode = blogUser.PostalCode;
                model.StripeUserId = blogUser.UserStripeId;
                model.CoverImageUrl = blogUser.CoverImageUrl;
                model.Address = $"{blogUser.Line1} {blogUser.City} {blogUser.Country}";
                model.Description = blogUser.Biography;
                model.PhoneNumber = blogUser.PhoneNumberPrefix + blogUser.PhoneNumber;
                model.Email = blogUser.Email;

                // Load the categories associated with the Blogger user
                if (blogUser != null)
                {
                    _context.Entry(blogUser)
                        .Collection(u => u.Categories)
                        .Load();
                }
                model.CategoryList = blogUser.Categories;
            }

            // Handle Company users
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var compUser = await _userManager.FindByIdAsync(id) as CompanyUser;

                // Populate model with Company user details
                model.Id = compUser.Id;
                model.SelectedRole = "Company";
                model.Name = compUser.Name;
                model.Address = compUser.Address;
                model.Description = compUser.Description;
                model.PhoneNumber = compUser.PhoneNumberPrefix + compUser.PhoneNumber;
                model.Email = compUser.Email;
                model.Url = compUser.Url;

                // Load the categories associated with the Company user
                if (compUser != null)
                {
                    _context.Entry(compUser)
                        .Collection(u => u.Categories)
                        .Load();
                }
                model.CategoryList = compUser.Categories;
            }

            return View(model);
        }

        // Method to upload an image to a specified folder
        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            // Create a unique file path with a GUID to prevent name clashes
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            // Combine the folder path with the web root path to get the full path
            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            // Copy the uploaded file to the server
            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            // Return the path for later use
            return "/" + folderPath;
        }

        // Action to delete a file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(string file)
        {
            // Define the directory where cover images are stored
            string fileDirectory = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/Images/cover");
            ViewBag.fileList = Directory
                .EnumerateFiles(fileDirectory, "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);
            ViewBag.fileDirectory = fileDirectory;

            // Construct the full path of the file to delete
            string webRootPath = _webHostEnvironment.WebRootPath;
            var fileName = file; // Store the filename for later use
            var fullPath = Path.Combine(webRootPath, "Images/cover", file);

            // Check if the file exists and delete it
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath); // Delete the file
                ViewBag.deleteSuccess = "true"; // Indicate successful deletion
            }

            // Redirect to the DeactivateConfirmed action after deletion
            return RedirectToAction("DeactivateConfirmed");
        }




    }



}
