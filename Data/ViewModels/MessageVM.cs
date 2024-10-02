using Instadvert.CZ.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Data.ViewModels
{
    public class MessageVM
    {
        

        public string Content { get; set; }
        public DateTime? SentAt { get; set; }

        public string SenderId { get; set; }
    
        public virtual List<Message>? Messages { get; set; }
        public string ReceiverId { get; set; }
        
       
    }
}
