using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace BettypeWith3rdProducts.Business.Services
{
    public class MailServiceImp : MailService
    {
        private readonly ExcelService excelService;
        private readonly MailContentBuilder mailContentBuilder;
        private readonly MailStationService mailStationService;
        private readonly ApplicationConfigurationService appConfig;

        public MailServiceImp(
            ExcelService excelService,
            MailContentBuilder mailContentBuilder,
            MailStationService mailStationService,
            ApplicationConfigurationService appConfig)
        {
            this.excelService = excelService;
            this.mailContentBuilder = mailContentBuilder;
            this.mailStationService = mailStationService;
            this.appConfig = appConfig;
        }

        public Mail Execute()
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

        public void SendMailWhenDataIsNotReady()
        {
            if (appConfig.FunctionIdNodata > 0)
            {
                var mail = mailContentBuilder.ReadContentWhenDataIsNotReady();
                MailClientHandler.Start();
                MailClientHandler.SendFunctionMail(mail);
                MailClientHandler.Stop();
            }
        }
    }
}