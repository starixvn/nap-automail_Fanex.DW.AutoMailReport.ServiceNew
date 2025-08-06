using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using Fanex.RocketChat.Client;
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

        Task SendMessageToRocketChat2(List<string> imagePaths);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private readonly RocketChatClientService rocketChatService;
        private readonly Log log;

        public MessageServiceImp(
            LetstalkClientService letstalkService,
            RocketChatClientService rocketChatService,
            Log log)
        {
            this.letstalkService = letstalkService;
            this.rocketChatService = rocketChatService;
            this.log = log;
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

            _ = SendMessageToRocketChat(imagePaths);

            return result;
        }

        public Task SendMessageToRocketChat2(List<string> images)
        {
            var files = images.Select(image => new RocketChat.Client.Models.FileModel
            {
                FileName = new FileInfo(image).Name,
                FileBytes = File.ReadAllBytes(image),
                MediaType = RocketChat.Client.Models.MediaType.Image
            }).ToList();

            AppConfigService.Config.RocketChat.NewChartChannels.ToList().ForEach(channel =>
            {
                var channelName = channel.Value.ChannelName;
                var messageRequest = new RocketChat.Client.Models.MessageRequest
                {
                    ChannelName = channelName,
                    Files = files,
                };

                rocketChatService.Send(messageRequest);
            });

            return Task.CompletedTask;
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

        private Task SendMessageToRocketChat(List<string> images)
        {
            var files = images.Select(image => new RocketChat.Client.Models.FileModel
            {
                FileName = new FileInfo(image).Name,
                FileBytes = File.ReadAllBytes(image),
                MediaType = RocketChat.Client.Models.MediaType.Image
            }).ToList();

            AppConfigService.Config.RocketChat.Channels.ToList().ForEach(channel =>
            {
                var channelName = channel.Value.ChannelName;
                var messageRequest = new RocketChat.Client.Models.MessageRequest
                {
                    ChannelName = channelName,
                    Files = files,
                };

                rocketChatService.Send(messageRequest);
            });

            return Task.CompletedTask;
        }
    }
}