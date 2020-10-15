using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface MailService
    {
        void SendMail(Mail mail);
    }

    public class MailServiceImp : MailService
    {
        public void SendMail(Mail mail)
        {
            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }
    }
}