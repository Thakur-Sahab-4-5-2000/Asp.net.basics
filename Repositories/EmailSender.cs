using Learning_Backend.Contracts;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MailKit.Security;

namespace Learning_Backend.Repositories
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = configuration.GetValue<string>("EmailSetting:SMTP_SERVER");
            _smtpPort = configuration.GetValue<int>("EmailSetting:SMTP_PORT");
            _smtpUsername = configuration.GetValue<string>("EmailSetting:SMTP_USERNAME");
            _smtpPassword = configuration.GetValue<string>("EmailSetting:SMTP_PASSWORD");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mimeMessage = new MimeMessage();
                string appName = _configuration.GetValue<string>("EmailSetting:APP_NAME");
                string emailSender  = _configuration.GetValue<string>("EmailSetting:EMAIL");
                mimeMessage.From.Add(new MailboxAddress(appName, emailSender));
                mimeMessage.To.Add(new MailboxAddress("Recipient Name", email));
                mimeMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = message };
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);

                    await client.SendAsync(mimeMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw new ApplicationException("An error occurred while sending the email.", ex);
            }
        }
    }
}
