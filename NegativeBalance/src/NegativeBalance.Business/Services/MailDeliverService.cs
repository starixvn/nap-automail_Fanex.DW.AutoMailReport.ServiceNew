using Fanex.MailDeliver.MailClient.API;
using Fanex.MailDeliver.Model;

namespace NegativeBalance.Business.Services
{
    public interface MailDeliverService
    {
        bool Send(MailRecord mail);
    }

    public class MailDeliverServiceImp : MailDeliverService
    {
        public bool Send(MailRecord mail)
        {
            if (string.IsNullOrEmpty(mail.MailTo))
            {
                return false;
            }

            using (var client = new MailDeliver())
            {
                client.SendMail(mail);
                return true;
            }
        }
    }
}