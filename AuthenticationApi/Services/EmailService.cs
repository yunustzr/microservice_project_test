using System.Net;
using System.Threading.Tasks;
using AuthenticationApi.Configurations;
using AuthenticationApi.Templates;
using Microsoft.Extensions.Options;
using MimeKit; 
using MailKit.Net.Smtp; 
using MailKit.Security; 

namespace AuthenticationApi.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string email, string verificationCode);
    }

    
    public class EmailService : IEmailService
    {
        private readonly SmtpConfig _smtpConfig;
        private readonly TemplateHelper _templateHelper;

        public EmailService(IOptions<SmtpConfig> smtpConfig,TemplateHelper templateHelper)
        {
            _smtpConfig = smtpConfig.Value;
            _templateHelper = templateHelper;
        }

        public async Task SendVerificationEmailAsync(string email, string verificationCode)
        {
            var verificationLink = $"http://localhost:7003/api/auth/verify-email?token={verificationCode}";

            var replacements = new Dictionary<string, string>
            {
                { "VerificationLink", verificationLink }
            };

            var emailBody = await _templateHelper.RenderTemplateAsync("VerificationEmail.html", replacements);
             
            // Email gönderme işlemi
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Autantication App", _smtpConfig.From));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Email Doğrulama";
            emailMessage.Body = new TextPart("html")
            {
                Text = emailBody
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpConfig.User, _smtpConfig.Pass);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }

    }
}
