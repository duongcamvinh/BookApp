using Api.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Api.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

            public async Task SendEmailAsync(string fromAddress, string toAddress, string subject, string messge)
        {
            var enable = bool.Parse(_config["Gmail:SMTP:StartTLS:Enable"].ToString());
            var mailMessage = new MailMessage(fromAddress, toAddress, subject, messge);

            var client = new SmtpClient(_config["Gmail:Host"], int.Parse(_config["Gmail:Port"]))
            {
                Credentials = new NetworkCredential(_config["Gmail:Username"], _config["Gmail:Password"]),
                EnableSsl = enable
            };
            {
                {
                    await client.SendMailAsync(mailMessage);
                }
            }
        }
    }
}
