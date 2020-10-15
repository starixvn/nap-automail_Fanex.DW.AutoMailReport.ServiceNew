using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(string filename, string group);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(string filename, string group)
        {
            var letstalkAttachment = new LetstalkAttachment
            {
                FileName = filename,
                Bytes = FileBinaryService.ConvertFileToBinary(filename)
            };

            var model = new LetstalkModel
            {
                TargetName = AppConfigService.AppConfig.TargetName,
                Group = group,
                LetstalkAttachments = new List<LetstalkAttachment> { letstalkAttachment },
                Subject = string.Empty,
                Content = string.Empty
            };

            return model;
        }
    }
}