using System;
using System.Collections.Generic;
using NegativeBalance.Business.Models;

namespace NegativeBalance.Business.Services
{
    public interface BalanceNotificationService
    {
        void Execute(bool isTest);
    }

    public class BalanceNotificationServiceImp : BalanceNotificationService
    {
        private readonly RepositoryBalanceService repository;
        private readonly DataService dataService;
        private readonly FileService fileService;
        private readonly RecipientService recipientService;
        private readonly MailDeliverService mailDeliverService;

        public BalanceNotificationServiceImp(
            RepositoryBalanceService repository,
            DataService dataService,
            FileService fileService,
            RecipientService recipientService,
            MailDeliverService mailDeliverService)
        {
            this.repository = repository;
            this.dataService = dataService;
            this.fileService = fileService;
            this.recipientService = recipientService;
            this.mailDeliverService = mailDeliverService;
        }

        public void Execute(bool isTest)
        {
            repository.QuerySites(isTest)
                .ForEach(site =>
                {
                    var data = dataService.QueryMatchesBySite(site);
                    bool isValidData = CheckValidData(data);

                    if (data.Item1.Count > 0 && isValidData)
                    {
                        var content = fileService.ReadContent(data.Item1);
                        var mail = recipientService.CreateMail(site, content);
                        var result = mailDeliverService.Send(mail);
                        if (result)
                        {
                            repository.UpdateMatchSent(data.Item2);
                        }
                    }
                });
        }

        private static bool CheckValidData(Tuple<List<Match>, string> data)
        {
            var isValidData = true;
            data.Item1.ForEach(item =>
            {
                if (!item.MatchInfo.IsValid)
                {
                    isValidData = false;
                }
            });

            return isValidData;
        }
    }
}