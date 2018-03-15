using System;
using System.Collections.Generic;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using MaxLossMonitoring.Business.Models;

namespace MaxLossMonitoring.Business.Services
{
    public interface MessageService
    {
        bool SendMessage(List<MonitorList> dataSource);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private readonly ApplicationConfigurationService appConfig;
        private readonly MessageContentService messageContentService;

        public MessageServiceImp(LetstalkClientService letstalkService, ApplicationConfigurationService appConfig, MessageContentService messageContentService)
        {
            this.letstalkService = letstalkService;
            this.appConfig = appConfig;
            this.messageContentService = messageContentService;
        }

        public bool SendMessage(List<MonitorList> dataSource)
        {
            var result = false;
            foreach (var group in dataSource)
            {
                var contentsOnEachGroup = messageContentService.GenerateMessageContent(group);
                foreach (var page in contentsOnEachGroup)
                {
                    var messsageSubject = $"Loss Monitor > {group.LevelNameForLetstalk}% {page.Key}of{contentsOnEachGroup.Count}";
                    var messageContent = string.Join(Environment.NewLine, page.Value);
                    var letstalkModel = CreateLetstalkMessageModel(messsageSubject, messageContent, null);
                    result = letstalkService.SendMessage(letstalkModel);
                }
            }

            return result;
        }

        private LetstalkModel CreateLetstalkMessageModel(string subject, string content, List<LetstalkAttachment> letstalkAttachments)
        {
            return new LetstalkModel
            {
                TargetName = appConfig.Configurations.TargetName,
                Subject = subject,
                Content = content,
                Group = appConfig.Configurations.Group,
                LetstalkAttachments = letstalkAttachments ?? new List<LetstalkAttachment>()
            };
        }
    }
}