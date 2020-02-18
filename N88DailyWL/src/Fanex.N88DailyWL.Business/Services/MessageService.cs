using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface MessageService
    {
        List<LetstalkModel> CreateLetstalkModel(List<string> filenames);
    }

    public class MessageServiceImp : MessageService
    {
        public List<LetstalkModel> CreateLetstalkModel(List<string> filenames)
        {
            var letstalkAttachments = new List<LetstalkAttachment>();
            var models = new List<LetstalkModel>();
            foreach (var filename in filenames)
            {
                letstalkAttachments.Add(new LetstalkAttachment
                {
                    FileName = filename,
                    Bytes = FileBinaryService.ConvertFileToBinary(filename)
                });
            }

            AppConfigService
                .AppConfig
                .Group
                .ForEach(group =>
                {
                    models.Add(new LetstalkModel
                    {
                        TargetName = AppConfigService.AppConfig.TargetName,
                        Group = group,
                        LetstalkAttachments = letstalkAttachments,
                        Subject = string.Empty,
                        Content = string.Empty
                    });
                });

            return models;
        }
    }
}