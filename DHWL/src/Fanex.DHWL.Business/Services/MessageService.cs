using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using Fanex.Mattermost.Client;
using Fanex.Mattermost.Client.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.DHWL.Business.Services
{
    public interface MessageService
    {
        Dictionary<string, bool> SendMessage(List<string> imagePaths, string letstalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private readonly MattermostClientService mattermostService;

        public MessageServiceImp(
            LetstalkClientService letstalkService,
            MattermostClientService mattermostService)
        {
            this.letstalkService = letstalkService;
            this.mattermostService = mattermostService;
        }

        public Dictionary<string, bool> SendMessage(List<string> imagePaths, string letstalkGroup)
        {
            var result = new Dictionary<string, bool>();
            imagePaths.ForEach(image =>
            {
                var letstalkModel = CreateLetstalkModel(image, letstalkGroup);
                var status = letstalkService.SendMessage(letstalkModel);
                Thread.Sleep(5000);
                const int maxRetry = 10;
                var retryCounter = 0;
                if (!status)
                {
                    while (retryCounter < maxRetry && !status)
                    {
                        status = letstalkService.SendMessage(letstalkModel);
                        retryCounter++;
                        Thread.Sleep(5000);
                    }
                }

                result.Add(new FileInfo(image).Name, status);
            });

            _ = SendMessageToMattermost(imagePaths);

            return result;
        }

        private async Task<bool> SendMessageToMattermost(List<string> images)
        {
            var files = images.Select(image => new FileModel
            {
                FileName = new FileInfo(image).Name,
                FileBytes = File.ReadAllBytes(image),
                MediaType = MediaType.Image,
            }).ToList();

            var matterMostChannel = AppConfigService.Config.Mattermost.Channels["PrePROD"];
            var messageRequest = new MessageRequest
            {
                ChannelId = matterMostChannel.ChannelId,
                Files = files,
            };
            var result = await mattermostService.Send(messageRequest);
            return result;
        }

        private static LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup)
        {
            var letstalkAttachments = new List<LetstalkAttachment>
            {
                new LetstalkAttachment
                {
                    FileName = fileName,
                    Bytes = FileBinaryService.ConvertFileToBinary(fileName)
                }
            };

            return new LetstalkModel
            {
                TargetName = AppConfigService.Config.TargetName,
                Group = letstalkGroup,
                LetstalkAttachments = letstalkAttachments,
                Subject = string.Empty,
                Content = string.Empty
            };
        }
    }
}