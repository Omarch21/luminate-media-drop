using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Runtime.CompilerServices;

namespace Luminate_media.services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
          public void SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(_config.GetSection("EmailReceiver").Value));
            email.Subject = request.Subject + " - Email From " + request.From;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<h2>Customer Name - "+ request.FirstName + " " + request.LastName + " </h2> <h4>Phone Number: "+ request.PhoneNumber + "</h4> <p>" + request.Body + "</p>" };
            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_config.GetSection("EmailUsername").Value, _config.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
            
        }
    }
}
