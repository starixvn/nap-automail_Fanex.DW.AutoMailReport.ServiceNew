using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface MailStationService
    {
        bool Send(Mail mail);
    }

    public class MailStationServiceImp : MailStationService
    {
        public bool Send(Mail mail)
        {
            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
            return true;
        }
    }
}