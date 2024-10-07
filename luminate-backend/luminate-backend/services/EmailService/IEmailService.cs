using Luminate_media.Models;

namespace Luminate_media.services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
