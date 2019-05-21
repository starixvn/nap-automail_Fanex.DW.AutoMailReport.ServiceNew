using System.Net;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;

namespace Fanex.ServiceMonitoring.Services
{
    public interface MessageService
    {
        void SendMessage();
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private readonly MessageContentService messageContentService;

        public MessageServiceImp(LetstalkClientService letstalkService, MessageContentService messageContentService)
        {
            this.letstalkService = letstalkService;
            this.messageContentService = messageContentService;
        }

        public void SendMessage()
        {
            var isAllow = IsAllowToSendMessage();
            if (!isAllow)
            {
                return;
            }

            var contents = messageContentService.GenerateMessage();
            foreach (var content in contents)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    var model = CreateLetstalkMessageModel(content);
                    letstalkService.SendMessage(model);
                }
            }
        }

        private LetstalkModel CreateLetstalkMessageModel(string content)
        {
            return new LetstalkModel
            {
                TargetName = AppConfigService.AppConfig.TargetName,
                Subject = $"{AppConfigService.AppConfig.Environment} - Mail Reports Issue",
                Content = content,
                Group = AppConfigService.AppConfig.LetstalkGroup
            };
        }

        private static bool IsAllowToSendMessage()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress addr in localIPs)
            {
                if (addr.ToString().Equals(AppConfigService.AppConfig.AllowIP))
                {
                    return true;
                }
            }

            return false;
        }
    }
}