using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.SuspiciousAccount.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.SuspiciousAccount.Business.Services
{
    public interface MessageService
    {
        bool SendNotifyStatus1(List<AccountModel> accounts);

        bool SendNotifyStatus2(List<AccountModel> accounts);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;

        public MessageServiceImp(LetstalkClientService letstalkService)
        {
            this.letstalkService = letstalkService;
        }

        public bool SendNotifyStatus1(List<AccountModel> accounts)
        {
            var content = new List<string>();
            content.Add($"1ST WARNING: {accounts.Count.ToString("#,##0")} users' credits >= RM 300K, evidence 100 flagged/affected.");

            accounts.ForEach(acc =>
            {
                acc.NotifyStatus1 = 2;
                content.Add($"{acc.UserName}: {acc.Currency}{acc.Credit / 1000: #,##0} k");
            });

            var letstalkModel = new LetstalkModel
            {
                TargetName = AppConfigService.Config.TargetName,
                Subject = $"Hi All",
                Content = string.Join(Environment.NewLine, content),
                Group = AppConfigService.Config.GroupName
            };

            var result = letstalkService.SendMessage(letstalkModel);
            return result;
        }

        public bool SendNotifyStatus2(List<AccountModel> accounts)
        {
            var content = new List<string>();
            content.Add($"2ND WARNING: {accounts.Count.ToString("#,##0")} potentially dangerous users has just placed bet");
            accounts.ForEach(acc =>
            {
                acc.NotifyStatus2 = 2;
                content.Add($"{acc.UserName}: {acc.Currency}{acc.Credit / 100: #,##0} k");
            });

            var letstalkModel = new LetstalkModel
            {
                TargetName = AppConfigService.Config.TargetName,
                Subject = $"Hi All",
                Content = string.Join(Environment.NewLine, content),
                Group = AppConfigService.Config.GroupName
            };

            var result = letstalkService.SendMessage(letstalkModel);
            return result;
        }
    }
}