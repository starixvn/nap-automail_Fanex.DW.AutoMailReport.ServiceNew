using System;
using System.Collections.Generic;
using Fanex.MailDelivery.Core.Models;
using TOControl.CustomerMovement.Business.Models;

namespace TOControl.CustomerMovement.Business.Services
{
    public interface MailMovementService
    {
        Mail Execute(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> data);
    }

    public class MailMovementServiceImp : MailMovementService
    {
        private readonly FileService fileService;
        private readonly AppConfig appConfig;
        private readonly DateService dateService;

        public MailMovementServiceImp(FileService fileService, ApplicationConfigurationService appConfigService, DateService dateService)
        {
            this.fileService = fileService;
            this.appConfig = appConfigService.Configurations;
            this.dateService = dateService;
        }

        public Mail Execute(Tuple<IEnumerable<GroupHeadline>, IEnumerable<Movement>, IEnumerable<DirectCustomer>> data)
        {
            var mail = new Mail
            {
                Subject = $"{appConfig.Subject} from {dateService.FromDate:MM/dd/yyyy} to {dateService.ToDate:MM/dd/yyyy}",
                Title = appConfig.Title,
                Body = fileService.ReadContent(data, dateService)
            };

            return mail;
        }
    }
}