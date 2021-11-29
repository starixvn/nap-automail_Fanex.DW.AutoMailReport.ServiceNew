using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface MailService
    {
        Mail CreateMail();
    }

    public class MailServiceImp : MailService
    {
        private readonly ExcelService excelService;
        private readonly MailContentBuilder mailContentBuilder;
        private readonly ApplicationConfigurationService appConfig;

        public MailServiceImp(
            ExcelService excelService,
            MailContentBuilder mailContentBuilder,
            ApplicationConfigurationService appConfig)
        {
            this.excelService = excelService;
            this.mailContentBuilder = mailContentBuilder;
            this.appConfig = appConfig;
        }

        public Mail CreateMail()
        {
            excelService.Execute();
            var fileName = excelService.Save();
            if (appConfig.FunctionIdWithoutAttachment > 0)
            {
                var mail = mailContentBuilder.ReadContentWithoutAttachment(fileName);
                MailClientHandler.Start();
                MailClientHandler.SendFunctionMail(mail);
                MailClientHandler.Stop();
            }

            return mailContentBuilder.ReadContentWithAttachment(fileName);
        }
    }
}