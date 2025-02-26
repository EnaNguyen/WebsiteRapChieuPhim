using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ProjectGSMAUI.Api.Utilities
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSection = _configuration.GetSection("SmtpSettings");
                string smtpServer = smtpSection["Server"];
                int smtpPort = int.Parse(smtpSection["Port"]);
                string smtpUsername = smtpSection["Username"];
                string smtpPassword = smtpSection["Password"];
                string fromEmail = smtpSection["FromEmail"];


                MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
                mailMessage.IsBodyHtml = true; 

                SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true; 

                smtpClient.Send(mailMessage);
                return true; 
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false; 
            }
        }
    }
}