using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;

namespace FastMarketWeeklyPerformance.Business.Services
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