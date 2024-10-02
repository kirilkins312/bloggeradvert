//using Instadvert.CZ.Data.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using Stripe.Identity;
//using Stripe;

//namespace Instadvert.CZ.Controllers
//{
//    public class чернетка
//    {
//        //

//        [HttpGet]
//        public IActionResult verification()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var options = new VerificationSessionCreateOptions
//            {
//                Type = "document",
//                ClientReferenceId = "acct_1PdgEIRsgoM1K78B",
//                Metadata = new Dictionary<string, string>
//  {
//    {"acct_1PdgEIRsgoM1K78B", "{{acct_1PdgEIRsgoM1K78B}}"},
//  },
//            };

//            var service = new VerificationSessionService();
//            var verificationSession = service.Create(options);

//            // Return only the client secret to the frontend.
//            var clientSecret = verificationSession.ClientSecret;
//            var model = new Verifid
//            {
//                ClientSecret = clientSecret,
//            };
//            return View(model);

//        }

//        public IActionResult Chekout()
//        {

//            var priceOptions = new PriceCreateOptions
//            {
//                Currency = "czk",
//                UnitAmount = 1000,
//                ProductData = new PriceProductDataOptions { Name = "Transaction" },
//            };
//            var priceService = new PriceService();
//            var price = priceService.Create(priceOptions);

//            var domain = "https://localhost:7211/";

//            var options = new Stripe.Checkout.SessionCreateOptions
//            {
//                SuccessUrl = "https://example.com/success",
//                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
//    {
//        new Stripe.Checkout.SessionLineItemOptions
//        {
//            Price = price.Id,
//            Quantity = 1,
//        },
//    },
//                Mode = "payment",
//            };
//            var service = new Stripe.Checkout.SessionService();
//            Session session = service.Create(options);
//            Response.Headers.Add("Location", session.Url);
//            return new StatusCodeResult(303);

//            //var service = new SessionService();
//            //Session session = service.Create(options);
//            //Response.Headers.Add("Location", session.Url);
//            //return new StatusCodeResult(303);

//        }



//        public IActionResult AccountLink()
//        {
//            //StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";
//            //var options = new AccountLinkCreateOptions
//            //{
//            //    Account = "acct_1PbnMHRpu4BS29jm",
//            //    RefreshUrl = "https://example.com/reauth",
//            //    ReturnUrl = "https://example.com/return",
//            //    Type = "account_onboarding",
//            //};
//            //var service = new AccountLinkService();
//            //service.Create(options);
//            //return View();

//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var service = new LoginLinkService();

//            service.Create("acct_1PdApKRqxKn0Dz1z");
//            return View();
//        }


//        public IActionResult CreateTransfer()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";
//            var options = new TransferCreateOptions
//            {
//                Amount = 40000,
//                Currency = "czk",
//                Destination = "acct_1PdaIVRpgCErdADb",

//            };
//            var service = new TransferService();
//            service.Create(options);
//            return View();
//        }

//        public IActionResult CreatePayout()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var options = new PayoutCreateOptions { Amount = 1100, Currency = "usd", Destination = "" };
//            var service = new PayoutService();
//            service.Create(options);
//            return View();
//        }

//        public async Task CreateUser()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp"; //right

//            // Create a new standard connected account.
//            var accountOptions = new AccountCreateOptions
//            {
//                BusinessProfile = new AccountBusinessProfileOptions
//                {
//                    Name = "Hamnoid",
//                    ProductDescription = "Cool ass shi",
//                    Url = "https://www.kebab.cz/",
//                    SupportPhone = "+420776352793",


//                },


//                Individual = new AccountIndividualOptions
//                {

//                    FirstName = "Kirio",
//                    LastName = "Kirio",
//                    Dob = new DobOptions
//                    {
//                        Day = 1,
//                        Month = 1,
//                        Year = 2000,
//                    },
//                    Email = "masdas@dsa.com",
//                    Phone = "+420776352793",
//                    Address = new AddressOptions
//                    {
//                        Line1 = "Sokolovska",
//                        City = "Praha",
//                        Country = "CZ",
//                        PostalCode = "13000"
//                    }

//                },
//                BusinessType = "individual",
//                Type = "standard",
//                Capabilities = new AccountCapabilitiesOptions
//                {
//                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
//                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true },
//                },
//            };

//            var service = new AccountService();
//            service.Create(accountOptions);




//        }
//        public async Task<IActionResult> CreateLinkAcc(string accountId)
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp"; //right



//            // Create a new account link to onboard the new account.
//            var options = new AccountLinkCreateOptions
//            {
//                Account = "acct_1PdaIVRpgCErdADb",
//                RefreshUrl = "http://localhost:4242/onboard-user/refresh",
//                ReturnUrl = "http://localhost:4242/success.html",
//                Type = "account_onboarding",
//            };
//            var service = new AccountLinkService();
//            var accountLink = service.Create(options);
//            return Redirect(accountLink.Url);

//        }

//        public async Task<IActionResult> AccountUpdateUI()
//        {

//            // Create a new account link to onboard the new account.
//            var options = new AccountLinkCreateOptions
//            {
//                Account = "acct_1PdaXFRrLyHFvZuZ",
//                RefreshUrl = "http://localhost:4242/onboard-user/refresh",
//                ReturnUrl = "http://localhost:4242/success.html",
//                Type = "account_update",
//            };
//            var service = new AccountLinkService();
//            var accountLink = service.Create(options);
//            return Redirect(accountLink.Url);

//        }

//        public IActionResult RetrieveAccount()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var service = new AccountService();


//            return View(service.Get("acct_1Nv0FGQ9RKHgCVdK"));
//        }



//        //public async Task CreateUser()
//        //{
//        //    StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";
//        //    var options = new AccountCreateOptions
//        //    {
//        //        Individual = new AccountIndividualOptions
//        //        {
//        //            FirstName = "Kiril",
//        //            LastName = "Iatseniuk",
//        //            Email = "kiril.iatseniuk@icloud.com",
//        //            Address = new AddressOptions
//        //            {
//        //                Country = "Czech Republic",
//        //                City = "Prague",
//        //                PostalCode = "19000",
//        //                Line1 = "sokolovska 1205"
//        //            },
//        //            Dob = new DobOptions
//        //            {
//        //                Day = 11,
//        //                Month = 1,
//        //                Year = 2000,
//        //            }


//        //        },

//        //        TosAcceptance = new AccountTosAcceptanceOptions
//        //        {
//        //            Date = DateTimeOffset.FromUnixTimeSeconds(1609798905).UtcDateTime,
//        //            Ip = "8.8.8.8",
//        //        },

//        //        Capabilities = new AccountCapabilitiesOptions
//        //        {
//        //           Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
//        //        },


//        //    BusinessType = "individual",
//        //        BusinessProfile = new AccountBusinessProfileOptions
//        //        {
//        //            Name = "Hamnoid",
//        //            ProductDescription = "Cool ass shi",


//        //        },

//        //        Country = "CZ",
//        //        Email = "kirilhunter8888@gmail.com065614",
//        //        Controller = new AccountControllerOptions
//        //        {


//        //            Fees = new AccountControllerFeesOptions { Payer = "application" },
//        //            Losses = new AccountControllerLossesOptions { Payments = "application" },
//        //            RequirementCollection = "application",
//        //            StripeDashboard = new AccountControllerStripeDashboardOptions
//        //            {
//        //                Type = "none",

//        //            },

//        //        },
//        //    };
//        //    var service = new AccountService();
//        //    service.Create(options);
//        //}


//        public async Task<string> CreateTokenAsync()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var options = new TokenCreateOptions
//            {
//                BankAccount = new TokenBankAccountOptions
//                {
//                    Country = "CZ",
//                    Currency = "czk",
//                    AccountHolderName = "Jenny Rosen",
//                    AccountHolderType = "individual",
//                    AccountNumber = "CZ6508000000192000145399",
//                },
//            };

//            var service = new TokenService();
//            Token token = await service.CreateAsync(options);
//            return token.Id;
//        }

//        public async Task AddBankAccountAsync(string token)
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var options = new ExternalAccountCreateOptions
//            {
//                ExternalAccount = token,
//            };

//            var service = new ExternalAccountService();
//            await service.CreateAsync("", options);
//        }

//        public async Task CreateAndAddBankAccount()
//        {
//            string token = await CreateTokenAsync();
//            await AddBankAccountAsync(token);
//        }


//        public async Task UpdateUser()
//        {
//            StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";

//            var options = new AccountUpdateOptions
//            {
//                Individual = new AccountIndividualOptions
//                {
//                    FirstName = "Kiril",
//                    LastName = "Iatseniuk",
//                    Email = "kiril.iatseniuk@icloud.com",
//                    Address = new AddressOptions
//                    {
//                        Country = "CZ",
//                        City = "Prague",
//                        PostalCode = "19000",
//                        Line1 = "sokolovska 1205"
//                    },
//                    Dob = new DobOptions
//                    {
//                        Day = 11,
//                        Month = 1,
//                        Year = 2000,
//                    }


//                },

//            };
//            var service = new AccountService();
//            service.Update("acct_1Pc5RxRtmSJH6Asq", options);
//        }



//        //public async Task CreateToken()
//        //{
//        //    StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";


//        //    var options = new TokenCreateOptions
//        //    {
//        //        BankAccount = new TokenBankAccountOptions
//        //        {
//        //            Country = "CZ",
//        //            Currency = "czk",
//        //            AccountHolderName = "Jenny Rosen",
//        //            AccountHolderType = "individual",

//        //            //AccountNumber = "1030300000002305997015",
//        //            AccountNumber = "CZ6508000000192000145399",
//        //        },
//        //    };
//        //    var service = new TokenService();
//        //     service.Create(options);


//        //}

//        //public async Task AddBankAccount()
//        //{
//        //    StripeConfiguration.ApiKey = "sk_test_51PLfmeRx0bw4sYxc82lIUApyIPnhsETKhrmxjj7NWZm0tBrd5cW8iHzyVc7BJSpTrE08DybyA10W2EA0vs3SrTcd00LeefgSIp";



//        //    var options = new ExternalAccountCreateOptions
//        //    {
//        //        ExternalAccount = "btok_1NAiJy2eZvKYlo2Cnh6bIs9c",
//        //    };

//        //    var service = new ExternalAccountService();
//        //    service.Create("acct_1Pc5RxRtmSJH6Asq", options); 
//        //}


//    }
//}
