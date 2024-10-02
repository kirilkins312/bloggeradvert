

using Instadvert.CZ.Data;
using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Instadvert.CZ.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;



        public ChatController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;

        }

           //<a class="me-2 nav-link-color" asp-action="DeleteMessages" asp-controller="Chat" asp-route-yourId="@id" asp-route-oppId="@item.Id">Delete Messages</a>
        public async Task<IActionResult> DeleteMessages(string yourId, string oppId)
        {
            if (oppId == yourId)
            {

                ViewBag.Message = "Something went wrong. Please, refresh the page";
                return View("Error");
            }
            if (oppId == null)
            {

                ViewBag.Message = "Interlocutor does not exist";
                return View("Error");
            }
            var messageVM = new MessageVM();
            var you = await _context.Users.FirstOrDefaultAsync(x => x.Id == yourId);
            if (you == null)
            {
                ViewBag.Message = "You need to Login";
                return View("Error");
            }
            var opp = await _userManager.FindByIdAsync(oppId);
            if (oppId == null)
            {
                ViewBag.Message = "User doesn't exist";
                return View("Error");
            }
            var messages = await _context.Messages
            .Where(x =>
            (x.SenderId == you.Id && x.ReceiverId == opp.Id) || // Messages sent by the user to the receiver
            (x.SenderId == opp.Id && x.ReceiverId == you.Id)) // Messages sent by the receiver to the user
            .OrderBy(x => x.SentAt) // order the messages by timestamp
            .ToListAsync();

            messageVM.SenderId = you.Id;
            messageVM.ReceiverId = opp.Id;


            // Check if there is an unread message
            if (messages != null)
            {
                foreach (var message in messages)
                {
                   
                        // Mark the message as read by setting its read property to true
                        _context.Remove(message);
                    
                }
                _context.SaveChanges();
            }

            if (messages.Count > 0)
            {
                if (messageVM.Messages == null)
                {
                    messageVM.Messages = new List<Message>();
                }

                // Now you can add items to messageVM.Messages
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        messageVM.Messages.Add(message);
                    }
                }

            }

            ViewBag.SenderId = you.Id;
            ViewBag.ReceiverId = opp.Id;
            ViewBag.senderName = you.UserName;
            ViewBag.receiverName = opp.UserName;

            return View(messageVM);

        }

        public async Task<IActionResult> Chat(string yourId, string oppId)
        {
            if (oppId == yourId)
            {

                ViewBag.Message = "Something went wrong. Please, refresh the page";
                return View("Error");
            }
            if (oppId == null)
            {

                ViewBag.Message = "Interlocutor does not exist";
                return View("Error");
            }
            var messageVM = new MessageVM();
            var you = await _context.Users.FirstOrDefaultAsync(x => x.Id == yourId);
            if (you == null)
            {
                ViewBag.Message = "You need to Login";
                return View("Error");
            }
            var opp = await _userManager.FindByIdAsync(oppId);
            if (oppId == null)
            {
                ViewBag.Message = "User doesn't exist";
                return View("Error");
            }
            var messages = await _context.Messages
            .Where(x =>
            (x.SenderId == you.Id && x.ReceiverId == opp.Id) || // Messages sent by the user to the receiver
            (x.SenderId == opp.Id && x.ReceiverId == you.Id)) // Messages sent by the receiver to the user
            .OrderBy(x => x.SentAt) // order the messages by timestamp
            .ToListAsync();

            messageVM.SenderId = you.Id;
            messageVM.ReceiverId = opp.Id;


            // Check if there is an unread message
            if (messages != null)
            {
                foreach (var message in messages.Where(x => x.read == false && x.ReceiverId == you.Id))
                {
                    if (message.read == false)
                    {
                        message.read = true;    // Mark the message as read by setting its read property to true
                        _context.Update(message);
                    }
                }
                _context.SaveChanges();
            }

            if (messages.Count > 0)
            {
                if (messageVM.Messages == null)
                {
                    messageVM.Messages = new List<Message>();
                }

                // Now you can add items to messageVM.Messages
                foreach (var message in messages)
                {
                    if (message != null)
                    {
                        messageVM.Messages.Add(message);
                    }
                }

            }

            ViewBag.SenderId = you.Id;
            ViewBag.ReceiverId = opp.Id;
            ViewBag.senderName = you.UserName;
            ViewBag.receiverName = opp.UserName;

            return View(messageVM);

        }

        public async Task<IActionResult> ChatList(string id)
        {


            if (id == null)
            {

                ViewBag.Message = "Something went wrong";
                ViewBag.SecondaryMesssage = "Try again later";
                return View("Error");
            }
            var user = await _context.Users.FirstOrDefaultAsync(x =>x.Id == id);
            if(User.IsInRole("Blogger"))
            {
                var blogUser = user as BloggerUser;
                if(blogUser.StripeAccActivated == false)
                {
                    ViewBag.Message = "You do not have an activated Stripe account.";
                    ViewBag.SecondaryMessage = "You can activate Stripe account in settings";
                    return View("Error");
                }

            }
            

            var chatsWithBloggers = _context.Users
            .OfType<User>()
                .Where(x => x.Id != user.Id && (x.Messages.Any(m => m.SenderId == user.Id || m.ReceiverId == user.Id) || _context.Messages.Any(m => m.SenderId == user.Id && m.ReceiverId == x.Id))).Include(x => x.Messages)
                .ToList(); // Find existing chats for user with other users, except the chat with himself.

            return View(chatsWithBloggers);



        }


    }
}
