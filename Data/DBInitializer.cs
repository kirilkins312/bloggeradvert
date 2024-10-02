using Instadvert.CZ.Controllers;
using Instadvert.CZ.Data.Static;
using Instadvert.CZ.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace Instadvert.CZ.Data
{
    public class DBInitializer //Database seed 
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            

            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Models.Category>()
                   {


                        new Models.Category()
                        {

                            Name = "Category 1",
                            Description = "Description for Category 1"


                        },
                       new Models.Category()
                       {

                           Name = "Category 2",
                           Description = "Description for Category 2"


                       },
                       new Models.Category()
                       {

                           Name = "Category 3",
                           Description = "Description for Category 3"


                       },
                       new Models.Category()
                       {

                           Name = "Category 4",
                           Description = "Description for Category 4"


                       },
                       new Models.Category()
                       {

                           Name = "Category 5",
                           Description = "Description for Category 5"


                       }
                    });
                }

                context.SaveChanges();
            }
        }

        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {

                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.Company))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Company));
                if (!await roleManager.RoleExistsAsync(UserRoles.Blogger))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Blogger));

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
                string adminUserEmail = "admin@rtx.com";

                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
                if (adminUser == null)
                {
                    var newAdminUser = new User()
                    {
                        PhoneNumberPrefix = "none",
                        Role = "Admin",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newAdminUser, "1234AbAb45sadsadAdas@");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                string compUserEmail = "toyota@icloud.com";

                var categories = context.Categories;

                var compUser = await userManager.FindByEmailAsync(compUserEmail);
                if (compUser == null)
                {
                    //context.Remove(compUser);
                    //context.SaveChanges();


                    var newCompUser = new CompanyUser()
                    {
                        UserName = "toyota.cz",
                        Role = "Company",
                        PhoneNumberPrefix = "+420",
                        Name = "Toyota",
                        Description = "The company designs, manufactures and sells passenger cars, buses, minivans, trucks, specialty cars, recreational and sport-utility vehicles. It provides financing to dealers and customers for the purchase or lease of vehicles.",
                        Address = "Aichi Prefecture 471-8571",
                        Url = "https://www.toyota.cz/",
                        Categories = categories.Where(x => x.CategoryId == 2 && x.CategoryId == 3).ToList(),
                        PhoneNumber = "776776776",
                        Email = compUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newCompUser, "1234AbAb45sadsadAdas@");
                    await userManager.AddToRoleAsync(newCompUser, UserRoles.Company);
                }
                string vwCompUserEmail = "vw@icloud.com";
                var vwCompUser = await userManager.FindByEmailAsync(vwCompUserEmail);
                if (vwCompUser == null)
                {
                    //context.Remove(compUser);
                    //context.SaveChanges();


                    var newVwCompUser = new CompanyUser()
                    {
                        UserName = "volkswagen.cz",
                        Role = "Company",
                        PhoneNumberPrefix = "+420",
                        Name = "Volkswagen",
                        Description = "The company designs, manufactures and sells passenger cars, buses, minivans, trucks, specialty cars, recreational and sport-utility vehicles. It provides financing to dealers and customers for the purchase or lease of vehicles.",
                        Address = " Aichi Prefecture 471-8571",
                        Url = "https://www.toyota.cz/",
                        Categories = categories.Where(x => x.CategoryId == 2 && x.CategoryId == 3).ToList(),
                        PhoneNumber = "776352793",
                        Email = vwCompUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newVwCompUser, "1234AbAb45sadsadAdas@");
                    await userManager.AddToRoleAsync(newVwCompUser, UserRoles.Company);
                }





                string blogUserEmail = "spiderman@icloud.com";
                var blogUser = await userManager.FindByEmailAsync(blogUserEmail);
                if (blogUser == null)
                {
                    //context.Remove(compUser);
                    //context.SaveChanges();


                    var newBlogUser = new BloggerUser()
                    {
                        UserName = "spider",
                        Role = "Blogger",
                        PhoneNumberPrefix = "+420",
                        Biography = "Spider-Man is a superhero in American comic books published by Marvel Comics. Created by writer-editor Stan Lee and artist Steve Ditko, he first appeared in the anthology comic book Amazing Fantasy #15 (August 1962) in the Silver Age of Comic Books. He has been featured in comic books, television shows, films, video games, novels, and plays.",
                        InstagramAvatar = "none",
                        InstagramFollowers = 10222222,
                        InstagramUsername = "spiderman",
                        CoverImageUrl = "/Images/initializePp/download.jpg",  
                        Categories = categories.Where(x => x.CategoryId == 2 && x.CategoryId == 3).ToList(),
                        PhoneNumber = "776352793",
                        Email = blogUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newBlogUser, "1234AbAb45sadsadAdas@");
                    await userManager.AddToRoleAsync(newBlogUser, UserRoles.Blogger);
                }

                string sBlogUserEmail = "batman@icloud.com";
                var sBlogUser = await userManager.FindByEmailAsync(sBlogUserEmail);
                if (sBlogUser == null)
                {
                    //context.Remove(compUser);
                    //context.SaveChanges();


                    var newSBlogUser = new BloggerUser()
                    {
                        UserName = "batman",
                        Role = "Blogger",
                        PhoneNumberPrefix = "+420",
                        Biography = "Batman[a] is a superhero who appears in American comic books published by DC Comics. Batman was created by the artist Bob Kane and writer Bill Finger, and debuted in the 27th issue of the comic book Detective Comics on March 30, 1939. In the DC Universe, Batman is the alias of Bruce Wayne, a wealthy American playboy, philanthropist, and industrialist who resides in Gotham City. His origin story features him swearing vengeance against criminals after witnessing the murder of his parents, Thomas and Martha, as a child, a vendetta tempered by the ideal of justice.",
                        InstagramAvatar = "none",
                        InstagramFollowers = 8888888,
                        InstagramUsername = "batmanDark",
                        CoverImageUrl = "/Images/initializePp/Batman_Infobox.jpg",
                        Categories = categories.Where(x => x.CategoryId == 2 && x.CategoryId == 3).ToList(),
                        PhoneNumber = "776352793",
                        Email = sBlogUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(newSBlogUser, "1234AbAb45sadsadAdas@");
                    await userManager.AddToRoleAsync(newSBlogUser, UserRoles.Blogger);
                }


            }

        }
    }
}