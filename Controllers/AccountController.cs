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
            var user = await _userManager.FindByIdAsync(Id);       
            var roles = new List<string>
            {
               UserRoles.Blogger,
               UserRoles.Company

            };
            ViewBag.Roles = roles;
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
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = user as BloggerUser;       
                var editBlogVM = new UserVM()
                {                                  
                    Email = blogUser.Email,
                    userRoles = roles,
                    SelectedRole = "Blogger"
                };
               return View(editBlogVM);
            }
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var compUser = user as CompanyUser;            
                var editCompVM = new UserVM()
                {
                    Email = compUser.Email,           
                    userRoles = roles,
                    SelectedRole = "Company",

                };
                return View(editCompVM);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(string id, UserVM model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }
            if (id != user.Id)
            {
                return NotFound();
            }
            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;
                blogUser.Email = model.Email;

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
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var companyUser = await _userManager.FindByIdAsync(id) as CompanyUser;
                companyUser.Email = model.Email;
                
             
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
                ViewBag.Message = "Succesfully edited";
                return View("Message");
            }
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
            if (DateTime.TryParse(Expiry, out DateTime expiryDate))
            {
                if (expiryDate < DateTime.UtcNow)
                {
                    ViewBag.Message = "This confirmation link has expired.";
                    return View("Error");
                }
            }
            
                DateTime newDate = new DateTime(2024, 5, 4);
            if (UserId == null || Token == null || expiryDate < newDate)
            {
                ViewBag.Message = "The link is Invalid or Expired";
            }
            //Find the User By Id
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }
            if (user.EmailConfirmed == true)
            {
                ViewBag.Message = "Email is already confirmed";
                return View("Error");
            }
          
            //Call the ConfirmEmailAsync Method which will mark the Email as Confirmed
            var result = await _userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                ViewBag.Message = "Thank you for confirming your email";
                return View();
            }
            ViewBag.Message = "Email cannot be confirmed";
            return View();
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
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            

            if (user != null)
            {
                if (user.AccountDeactivated == true)
                {
                    ViewBag.Message = "User does not exist";
                    return View("Error");
                }
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    if (user.EmailConfirmed == true)
                    {
                        if(user.TwoFactorCustomEnabled == true)
                        {
                            string email = user.Email;
                            //sending code
                            var model = await SendVerificationCodeByEmail(email);
                            // inserting data to view model which we will use for verification
                            model.Email = email;
                            model.Password = loginVM.Password;
                            return View("VerifyVerificationCodeGet", model);
                        }
                        else
                        {
                            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                            if (result.Succeeded)
                            {

                                ViewBag.Message = "Congratulations";
                                ViewBag.SecondaryMessage = "You are now loginned";
                                return View("Message");
                            }

                            return View("Error", ViewBag.Message = "Something went wrong, try again later");
                        }
                          
                    }
                    return View("RegistrationSuccessful");
                }
                ViewBag.Message = "Wrong credentials, try again";
                return View(loginVM); 
            }
            ViewBag.Message = "Wrong credentials, try again";
            return View(loginVM);
        }

        public IActionResult Registration()
        {
            var roles = new List<string>
            {
               UserRoles.Blogger,
               UserRoles.Company
               
            };
            ViewBag.Roles = roles;

            var categories =  _context.Categories.ToList();
            var model = new Data.ViewModels.UserVM();

            if (categories.Count > 0)
            {

                model.CategoryList = categories;
                model.userRoles = roles;
               
            }
            else
            {
                return View("Error", ViewBag.Message = "Something went wrong, try again later");
            }
            
           
            ViewBag.Categories = categories;
            return View(model);
        }
           

        [HttpPost]
        public async Task<IActionResult> Registration(UserVM signInVM)
        {
            var roles = _context.UserRoles.ToList();
            ViewBag.Roles = roles;


            var user = await _userManager.FindByEmailAsync(signInVM.Email);
            if (user != null)
            {
                TempData["Error"] = "This email is already taken";
                return RedirectToAction("Registration");
            }
            
                if(signInVM.SelectedRole == "Blogger")
                {
                    if (signInVM.CoverPhoto != null)
                    {
                        string folder = "Images/cover/";
                        signInVM.CoverImageUrl = await UploadImage(folder, signInVM.CoverPhoto);
                    }

                    var newUser = new BloggerUser()
                    {
                        //FullName = signInVM.FullName,
                        Email = signInVM.Email,
                        InstagramUsername = signInVM.InstagramUsername,
                        //DateOfBirth = signInVM.DateOfBirth,
                        InstagramAvatar = signInVM.InstagramAvatar,
                        InstagramFollowers = signInVM.InstagramFollowers,
                        //Address = signInVM.Address,
                        UserName = signInVM.UserName,
                        Name = signInVM.UserName,
                        PhoneNumber =  signInVM.PhoneNumber,
                        PhoneNumberPrefix = signInVM.PhoneNumberPrefix,
                        CoverImageUrl = signInVM.CoverImageUrl,
                        Biography = signInVM.Biography,

                        Role = signInVM.SelectedRole,

                    };


                    foreach (var categoryId in signInVM.SelectedCategories)
                    {
                        var selectedCategory = _context.Categories.Find(categoryId);
                        if (selectedCategory != null)
                        {
                            newUser.Categories.Add(selectedCategory);
                        }
                    }


                    var newUserResponse = await _userManager.CreateAsync(newUser, signInVM.Password);

                    if (newUserResponse.Succeeded)
                    {

                        await SendConfirmationEmail(newUser.Email, newUser);

                        await _userManager.AddToRoleAsync(newUser, signInVM.SelectedRole);
                        return View("RegistrationSuccessful");
                    }
                    else
                    {
                        foreach (var error in newUserResponse.Errors)
                         {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                     }


                    
                }
                if (signInVM.SelectedRole == "Company")
                {
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
                    Role = signInVM.SelectedRole,
                };


                    foreach (var categoryId in signInVM.SelectedCategories)
                    {
                        var selectedCategory = _context.Categories.Find(categoryId);
                        if (selectedCategory != null)
                        {
                            newUser.Categories.Add(selectedCategory);
                        }
                    }


                    var newUserResponse = await _userManager.CreateAsync(newUser, signInVM.Password);

                    if (newUserResponse.Succeeded)
                    {
                        await SendConfirmationEmail(newUser.Email, newUser);

                        await _userManager.AddToRoleAsync(newUser, signInVM.SelectedRole);
                    }
                    return View("RegistrationSuccessful");
                }
            


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

 public async Task<IActionResult> BloggerList()
        {
            var categories =  _context.Categories.ToList();

            if (categories == null)
            {
                return NotFound();
            }

            var model = new FilterVM();

            var blogUsersWithCategories = _context.Users.OfType<BloggerUser>().ToList();
            foreach (var user in blogUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            model.blogUsers = blogUsersWithCategories.ToList();
            model.CategoryList = categories;    

            return View(model);
        }
        public async Task<IActionResult> CompanyList()
        {
            var categories = _context.Categories.ToList();

            if (categories == null)
            {
                return NotFound();
            }

            var model = new FilterVM();

            var compUsersWithCategories = _context.Users.OfType<CompanyUser>().ToList();
            foreach (var user in compUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            model.compUsers = compUsersWithCategories.ToList();
            model.CategoryList = categories;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> BloggerList(FilterVM model)
        {
            var categories = _context.Categories.ToList();            

            if (categories == null)
            {
                return NotFound();
            }

            var blogUsersWithCategories = _context.Users.OfType<BloggerUser>().ToList();
            foreach (var user in blogUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            ///////FILTERS////

            if (model.SelectedCategories != null)
            {
                foreach (var categoryId in model.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);
                    if (selectedCategory != null)
                    {
                        blogUsersWithCategories = blogUsersWithCategories.Where(b => b.Categories.Any(c => c.CategoryId == categoryId)).ToList();
                        //Cheking selected categories ids and filtering list
                    }
                }
            }

            if (model.searchString != null)
            {
                model.searchString = model.searchString.ToLower();
                //blogUsersWithCategories = blogUsersWithCategories.Where(x => x.FullName.ToLower().Contains(model.searchString)).ToList();
            }
            if (model.searchStringTop != null && model.searchStringTop != 0)
            {
                blogUsersWithCategories = blogUsersWithCategories.Where(x => x.InstagramFollowers < model.searchStringTop).ToList();
            }
            if (model.searchStringBottom != null && model.searchStringBottom != 0)
            {
                blogUsersWithCategories = blogUsersWithCategories.Where(x => x.InstagramFollowers > model.searchStringBottom).ToList();
            }
            ////////////////////////////////
            model.blogUsers = blogUsersWithCategories;
            model.CategoryList = categories;

            ViewBag.Categories = categories;

            return View(model);
          
        }
    
        [HttpPost]
        public async Task<IActionResult> CompanyList(FilterVM model)
        {
            var categories = _context.Categories.ToList();

            if (categories == null)
            {
                return NotFound();
            }

            var compUsersWithCategories = _context.Users.OfType<CompanyUser>().ToList();
            foreach (var user in compUsersWithCategories)
            {
                await _context.Entry(user)
                        .Collection(u => u.Categories)
                        .LoadAsync();
            }

            if (model.SelectedCategories != null)
            {
                foreach (var categoryId in model.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);
                    if (selectedCategory != null)
                    {
                        compUsersWithCategories = compUsersWithCategories.Where(b => b.Categories.Any(c => c.CategoryId == categoryId)).ToList();
                        //Cheking selected categories ids and filtering list
                    }
                }
            }

            if (model.SelectedCategories != null)
            {
                foreach (var categoryId in model.SelectedCategories)
                {
                    var selectedCategory = _context.Categories.Find(categoryId);
                    if (selectedCategory != null)
                    {
                        compUsersWithCategories = compUsersWithCategories.Where(b => b.Categories.Any(c => c.CategoryId == categoryId)).ToList();
                        //Cheking selected categories ids and filtering list
                    }
                }
            }

            if (model.searchString != null)
            {
                model.searchString = model.searchString.ToLower();
                compUsersWithCategories = compUsersWithCategories.Where(x => x.Name.ToLower().Contains(model.searchString)).ToList();
            }

            model.compUsers = compUsersWithCategories;
            model.CategoryList = categories;

            ViewBag.Categories = categories;

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
            var user = await _userManager.FindByIdAsync(id);

            if(user.TwoFactorCustomEnabled == true)
            { 
                user.TwoFactorCustomEnabled = false;
                ViewBag.Message = "Two factor authentication disabled";

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();


                }
                return View("Error");
            }
            if (user.TwoFactorCustomEnabled == false)
            {
                user.TwoFactorCustomEnabled = true;

                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();


                }
                ViewBag.Message = "Two factor authentication enabled";
                return View("Error");
            }

          
           return RedirectToAction("Home","Index");
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            ////////////////////////////////////////////////////////////ROLES

            var user = await _userManager.FindByIdAsync(Id);

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;
            ViewBag.Id = user.Id;
            var roles = new List<string>
            {
               UserRoles.Blogger,
               UserRoles.Company

            };
            ViewBag.Roles = roles;
            if(user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            if (Id != user.Id)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {

                var blogUser = user as BloggerUser;
                await _context.Entry(blogUser).Collection(b => b.Categories).LoadAsync();
                var bloggersCategories = blogUser.Categories;

                foreach (var category in categories)
                {
                    category.IsChecked = bloggersCategories.Any(x => x.CategoryId == category.CategoryId);
                }

                var editBlogVM = new UserVM()
                {
                    Id= blogUser.Id,
                    
                    UserName = blogUser.UserName,
                    CategoryList = categories,
                    //FullName = blogUser.FullName,
                    Email = blogUser.Email,
                    PhoneNumber = blogUser.PhoneNumber,
                    PhoneNumberPrefix = blogUser.PhoneNumberPrefix,
                    //Address = blogUser.Address,
                    //DateOfBirth = blogUser.DateOfBirth,
                    InstagramAvatar = blogUser.InstagramAvatar,
                    InstagramFollowers = blogUser.InstagramFollowers,
                    InstagramUsername = blogUser.InstagramUsername,
                    Biography = blogUser.Biography,

                    Role= blogUser.Role,
                    SelectedRole = "Blogger"

                };

                return View(editBlogVM);

            }


            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var compUser = user as CompanyUser;
                await _context.Entry(compUser).Collection(b => b.Categories).LoadAsync();

                var companiesCategories = compUser.Categories;

                foreach (var category in categories)
                {
                    category.IsChecked = companiesCategories.Any(x => x.CategoryId == category.CategoryId);
                }

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
                    Url = compUser.Url,
                };
            

                return View(editCompVM);

            }

            return NotFound();



        }

   

        // POST: Bloggers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserVM model)
        {

            var user = await _userManager.FindByIdAsync(id);
            ViewBag.Id = model.Id;
            if (user == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

           

            if (id != user.Id)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;

                if (blogUser != null)
                {
                    await _context.Entry(blogUser)
                        .Collection(u => u.Categories)
                        .LoadAsync();
                }

                blogUser.UserName = model.UserName;
                //blogUser.FullName = model.FullName;
                blogUser.PhoneNumberPrefix = model.PhoneNumberPrefix;
                blogUser.PhoneNumber = model.PhoneNumber;
                //blogUser.Address = model.Address;
                //blogUser.DateOfBirth = model.DateOfBirth;
                blogUser.InstagramAvatar = model.InstagramAvatar;
                blogUser.InstagramFollowers = model.InstagramFollowers;
                blogUser.InstagramUsername = model.InstagramUsername;
                blogUser.Biography = model.Biography;

                blogUser.Categories.Clear();

                foreach (var categoryId in model.SelectedCategories)
                {
                    var category = _context.Categories.Find(categoryId);
                    if (category != null)
                    {
                        blogUser.Categories.Add(category);
                    }
                }           
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
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var companyUser = await _userManager.FindByIdAsync(id) as CompanyUser;

                if (companyUser != null)
                {
                    await _context.Entry(companyUser)
                        .Collection(u => u.Categories)
                        .LoadAsync();
                }

                

                companyUser.UserName = model.UserName;
                companyUser.Name = model.Name;
                companyUser.PhoneNumberPrefix = model.PhoneNumberPrefix;
                companyUser.PhoneNumber = model.PhoneNumber;
                companyUser.Address = model.Address;
                companyUser.Description = model.Description;
                companyUser.Url = model.Url;
                companyUser.Categories.Clear();

                foreach (var categoryId in model.SelectedCategories)
                {
                    var category = _context.Categories.Find(categoryId);
                    if (category != null)
                    {
                        companyUser.Categories.Add(category);
                    }
                }
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
                ViewBag.Message = "Succesfully edited";
                return View("Message");


            }
            ViewBag.Message = "Something went wrong";
            return View("Error");
        }
           
            public IActionResult Deactivate(string id)
            {

            return View("Deactivate", id);
            }



        public async Task<IActionResult> DeactivateConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            //First Fetch the User you want to Delete

            if (user == null)
            {
                // Handle the case where the user wasn't found
                ViewBag.Message = "User can not be found.";
                return View("Error");
            }
            else if (user.AccountDeactivated == true)
            {
                // Handle the case where the user wasn't found
                ViewBag.Message = "Account is already deactivated.";
                return View("Error");
            }
           

                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Blogger"))
                    {
                        ///DELETE USELESS INFO

                        var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;
                        blogUser.InstagramAvatar = "";
                        blogUser.InstagramFollowers = 0;
                        blogUser.InstagramUsername = "";
                        //blogUser.Address = "";
                        blogUser.EmailConfirmed = false;
                        DeleteFile(blogUser.CoverImageUrl);
                        blogUser.CoverImageUrl = "";
                    blogUser.Biography = "";
                    blogUser.BussinessName= "";
                    blogUser.City= "";
                    blogUser.Country = "";
                    blogUser.Day= null;
                    blogUser.FirstName= "";
                    blogUser.LastName = "";
                    blogUser.Line1 = "";
                    blogUser.Month= null;
                    blogUser.PostalCode= null;
                    blogUser.ProductDescription = "";
                    blogUser.Url = "";
                    blogUser.Year = null;

                    blogUser.AccountDeactivated = true;
                        blogUser.DeactivatedAt = DateTime.Now;

                        _context.Update(blogUser);
                        await _context.SaveChangesAsync();
                        if (user.Id == User.Identity.GetUserId())
                        {
                            await _signInManager.SignOutAsync();
                        }
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Company"))
                    {
                    ///DELETE USELESS INFO
                    var compUser = await _userManager.FindByIdAsync(id) as CompanyUser;
                        compUser.Description = "";
                        compUser.Address = "";
                        compUser.PhoneNumber = "";
                        compUser.EmailConfirmed = false;
                        compUser.Url = "";
                        compUser.AccountDeactivated = true;
                        compUser.DeactivatedAt = DateTime.Now;

                        _context.Update(compUser);
                        await _context.SaveChangesAsync();
                        if (user.Id == User.Identity.GetUserId())
                        {
                            await _signInManager.SignOutAsync();
                        }
                    }
                    


                }

                ViewBag.Message = "Succesfully deactivated";
                return View("Message");
            
        }

        

        public IActionResult Delete(string id)
        {

            return View("Delete", id);
        }
        public async Task<IActionResult> DeleteConfirmed(string id)
            {
            
            var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                     ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                     return View("Error");
                }
                  //var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6); ///// change to -6 or smaller


            if(user.AccountDeactivated == false)
            {
                ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                return View("Error");
            }
            if (user.DeactivatedAt == null)
            {
                ViewBag.Message = "Can not delete this account yet. Deactivate it.";
                 return View("Error");
            }
            //if (user.DeactivatedAt.Value <= sixMonthsAgo)
            //{

            //First Fetch the User you want to Delete



            var transactions = _context.Transactions.Where(x => x.TransactionCompanyUserId == id).ToList();
            foreach(var item in transactions)
            {

                item.TransactionCompanyUserId = id;
                item.Blogger = null;
                
            }


            var messagesSent =  _context.Messages.Where(m => m.SenderId == user.Id).ToList();
                    foreach (var message in messagesSent)
                    {
                       _context.Messages.Remove(message);
                    }

                    // Remove relationships where the user is the receiver
                    var messagesReceived =  _context.Messages.Where(m => m.ReceiverId == user.Id).ToList();
                    foreach (var message in messagesReceived)
                    {
                    _context.Messages.Remove(message);
                }

                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        if(user.Id == User.Identity.GetUserId())
                {
                    await _signInManager.SignOutAsync();
                }
                        
                        // Handle a successful delete
                        return View("DeleteConfirmed");
                    }
                    else
                    {
                        // Handle failure
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    return View();
                
            //}

           

        }

        public async Task<IActionResult> Details(string id)
            {

            if(id == null)
            {
                ViewBag.Message = "User does not exist";
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(id);



            var model = new UserVM()
            {
                TwoFactorCustomEnabled = user.TwoFactorCustomEnabled,
                AccountDeactivated = user.AccountDeactivated,
                DeactivatedAt = user.DeactivatedAt,
               
                
            };

            


            if (user == null)
            {
                TempData["Error"] = "User doesnt exist";
                RedirectToAction("Index", "Home");
            }
            

            if (await _userManager.IsInRoleAsync(user, "Blogger"))
            {
                var blogUser = await _userManager.FindByIdAsync(id) as BloggerUser;

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
                

                if (blogUser != null)
                {
                    _context.Entry(blogUser)
                        .Collection(u => u.Categories)
                        .Load();
                }
                model.CategoryList = blogUser.Categories;

            }
            if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                var compUser = await _userManager.FindByIdAsync(id) as CompanyUser;

                model.Id = compUser.Id;
                model.SelectedRole = "Company";
                model.Name = compUser.Name;
                model.Address = compUser.Address;
                model.Description = compUser.Description;
                model.PhoneNumber = compUser.PhoneNumberPrefix + compUser.PhoneNumber;
                model.Email = compUser.Email;
                model.Url = compUser.Url;

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


        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {
            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(string file)
        {
            string fileDirectory = Path.Combine(
                      Directory.GetCurrentDirectory(), "wwwroot/Images/cover");
            ViewBag.fileList = Directory
                .EnumerateFiles(fileDirectory, "*", SearchOption.AllDirectories)
                .Select(Path.GetFileName);
            ViewBag.fileDirectory = fileDirectory;
            string webRootPath = _webHostEnvironment.WebRootPath;
            var fileName = "";
            fileName = file;
            var fullPath = webRootPath + "/Images/cover/" + file;

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                ViewBag.deleteSuccess = "true";
            }
            return RedirectToAction("DeactivateConfirmed");
        }



        /////MANUALLY CONFRIM EMAIL///////////////////
        //public async Task<IActionResult> Conf(string UserId)
        //{
        //    var user = await _userManager.FindByIdAsync(UserId);
        //    //Generate the Token
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
           
            
        //    //Find the User By Id
          
        //    if (user == null)
        //    {
        //        ViewBag.ErrorMessage = $"The User ID {UserId} is Invalid";
        //        return View("NotFound");
        //    }
        //    //Call the ConfirmEmailAsync Method which will mark the Email as Confirmed
        //    var result = await _userManager.ConfirmEmailAsync(user, token);
        //    if (result.Succeeded)
        //    {
        //        ViewBag.Message = "Thank you for confirming your email";
        //        return View();
        //    }
        //    ViewBag.Message = "Email cannot be confirmed";
        //    return View();
        //}

        //public IActionResult TEST()
        //{
        //    return View();
        //}

    }



}
