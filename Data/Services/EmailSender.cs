
using System.Net.Mail;
using System.Net;

namespace Instadvert.CZ.Data.Services
{
    public class EmailSender : ISenderEmail
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // Sending email 
        public Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            string mail = "";
            var pw = "";
           
            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                Credentials = new NetworkCredential(mail, pw),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage(mail, ToEmail, Subject, Body)
            {
                IsBodyHtml = IsBodyHtml
            };
            return client.SendMailAsync(mailMessage);
        }

        public Task SendEmaiVerificationlAsync(string email, string Subject, string message)
        {

            string mail = "";
            var pw = "";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                Credentials = new NetworkCredential(mail, pw),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage(mail, email, Subject, message);
           
            return client.SendMailAsync(mailMessage);
        }
    }
}
