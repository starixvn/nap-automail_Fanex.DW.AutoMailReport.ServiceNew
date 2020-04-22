using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(string fileName);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(string fileName)
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
                Group = AppConfigService.Config.Group,
                LetstalkAttachments = letstalkAttachments,
                Subject = string.Empty,
                Content = string.Empty
            };
        }
    }
}