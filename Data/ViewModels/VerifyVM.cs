using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Instadvert.CZ.Data.ViewModels
{
    public class VerifyVM
    {
        public string Password { get; set; }
        [DisplayName("Code:")]
        public string UserCode{ get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        public string Code { get; set; }
        public DateTime Expiry { get; set; }    
    }
}
