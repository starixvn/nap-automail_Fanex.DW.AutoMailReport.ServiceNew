using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;
using System.IO;

namespace Fanex.DHWL.Business.Services
{
    public interface MessageService
    {
        Dictionary<string, bool> SendMessage(List<string> imagePaths, string letstalkGroup);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;

        public MessageServiceImp(LetstalkClientService letstalkService)
        {
            this.letstalkService = letstalkService;
        }

        public Dictionary<string, bool> SendMessage(List<string> imagePaths, string letstalkGroup)
        {
            var result = new Dictionary<string, bool>();
            imagePaths.ForEach(image =>
            {
                var letstalkModel = CreateLetstalkModel(image, letstalkGroup);
                var f = letstalkService.SendMessage(letstalkModel);
                result.Add(new FileInfo(image).Name, f);
            });

            return result;
        }

        private LetstalkModel CreateLetstalkModel(string fileName, string letstalkGroup)
        {
            var letstalkAttachments = new List<LetstalkAttachment>();
            letstalkAttachments.Add(new LetstalkAttachment
            {
                FileName = fileName,
                Bytes = FileBinaryService.ConvertFileToBinary(fileName)
            });

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