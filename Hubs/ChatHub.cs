using Instadvert.CZ.Controllers;
using Instadvert.CZ.Data;
using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Instadvert.CZ.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        public ChatHub(ApplicationDbContext context)
        {
          
            _context = context;

        }


        public async Task SendMessage(string receiver, string user, string message)
        {
            var messageModel = new Message()
            {
                SenderId = user,
                ReceiverId = receiver,
                Content = message,
                SentAt = DateTime.Now,
            };
            
            

             await Clients.User(receiver).SendAsync("ReceiveMessage", user, message);
            await Clients.User(user).SendAsync("ReceiveMessage", user, message);

            _context.Messages.Add(messageModel);
            await _context.SaveChangesAsync();
        }   


    }
}
