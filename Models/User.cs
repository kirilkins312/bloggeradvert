using Instadvert.CZ.Data;
using Microsoft.AspNetCore.Identity;

namespace Instadvert.CZ.Models
{
    public class User : IdentityUser
    {
        
        public string Role { get; set; }
        public string PhoneNumberPrefix{ get; set; }
        public virtual bool TwoFactorCustomEnabled {  get; set; } = false;
        public virtual bool AccountDeactivated { get; set; } = false;
        public DateTime? DeactivatedAt { get; set; }
        public DateTime? EmailChanged { get; set; }
        public DateTime? PhonenumberChanged { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        //Если требуется связь с транзакциями
       


        public User()
        {
            Messages = new HashSet<Message>();
          

        }

        
    }
}
