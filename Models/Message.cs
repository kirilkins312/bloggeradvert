using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instadvert.CZ.Models
{
    public class Message
    {
        [Key]
        public string Id { get; set; }
            
            public string Content { get; set; }
            public DateTime SentAt { get; set; }
            public bool read {  get; set; } = false;

            public string? SenderId { get; set; }
            [ForeignKey("SenderId")]
            public virtual User Sender { get; set; }

            public string? ReceiverId { get; set; }
            [ForeignKey("ReceiverId")]
            public virtual User Receiver { get; set; }

        
    }
}
