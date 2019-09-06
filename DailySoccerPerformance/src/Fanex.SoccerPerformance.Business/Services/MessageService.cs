using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface MessageService
    {
        LetstalkModel CreateLetstalkModel(List<string> filenames);
    }

    public class MessageServiceImp : MessageService
    {
        public LetstalkModel CreateLetstalkModel(List<string> filenames)
        {
            var letstalkAttachments = new List<LetstalkAttachment>();
            foreach (var filename in filenames)
            {
                letstalkAttachments.Add(new LetstalkAttachment
                {
                    FileName = filename,
                    Bytes = FileBinaryService.ConvertFileToBinary(filename)
                });
            }

            return new LetstalkModel
            {
                TargetName = AppConfigService.AppConfig.TargetName,
                Group = AppConfigService.AppConfig.Group,
                LetstalkAttachments = letstalkAttachments,
                Subject = "Soccer Performance Report",
                Content = "This is auto-generated content by system."
            };
        }
    }
}
