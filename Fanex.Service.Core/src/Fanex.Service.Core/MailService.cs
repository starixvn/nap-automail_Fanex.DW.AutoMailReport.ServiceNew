using Fanex.MailDelivery.Core.Models;

namespace Fanex.Service.Core
{
    public interface MailService
    {
        Mail Execute();

        void SendMailWhenDataIsNotReady();
    }
}