namespace Instadvert.CZ.Data.Services
{
    public interface ISenderEmail
    {
        Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false);
        Task SendEmaiVerificationlAsync(string email, string Subject, string message);
    }
}
