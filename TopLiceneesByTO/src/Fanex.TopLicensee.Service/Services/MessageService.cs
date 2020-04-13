using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using System.Collections.Generic;

namespace Fanex.TopLicensee.Service.Services
{
    public interface MessageService
    {
        bool Send(List<string> files);

        bool SendToLicenseeVirtualGroup(List<string> files);
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;

        public MessageServiceImp(LetstalkClientService letstalkService)
        {
            this.letstalkService = letstalkService;
        }

        public bool Send(List<string> files)
        {
            var messsageSubject = string.Empty;
            var messageContent = string.Empty;
            var letstalkAttachments = CreateAttachments(files);
            var letstalkModel = CreateLetstalkMessageModel(messsageSubject, messageContent, letstalkAttachments);
            var result = letstalkService.SendMessage(letstalkModel);
            return result;
        }

        private LetstalkModel CreateLetstalkMessageModel(string subject, string content, List<LetstalkAttachment> letstalkAttachments)
        {
            return new LetstalkModel
            {
                TargetName = AppConfigService.AppConfig.TargetName,
                Subject = subject,
                Content = content,
                Group = AppConfigService.AppConfig.Group,
                LetstalkAttachments = letstalkAttachments ?? new List<LetstalkAttachment>()
            };
        }

        private LetstalkModel CreateLetstalkMessageModelLicenseeVirtualGroup(string subject, string content, List<LetstalkAttachment> letstalkAttachments)
        {
            return new LetstalkModel
            {
                TargetName = AppConfigService.AppConfig.TargetName,
                Subject = subject,
                Content = content,
                Group = AppConfigService.AppConfig.LicenseeVirtualGroup,
                LetstalkAttachments = letstalkAttachments ?? new List<LetstalkAttachment>()
            };
        }

        private List<LetstalkAttachment> CreateAttachments(List<string> files)
        {
            var attachments = new List<LetstalkAttachment>();
            files.ForEach(file =>
            {
                attachments.Add(new LetstalkAttachment
                {
                    FileName = file,
                    Bytes = FileBinaryService.ConvertFileToBinary(file)
                });
            });

            return attachments;
        }

        public bool SendToLicenseeVirtualGroup(List<string> files)
        {
            var messsageSubject = string.Empty;
            var messageContent = string.Empty;
            var letstalkAttachments = CreateAttachments(files);
            var letstalkModel = CreateLetstalkMessageModelLicenseeVirtualGroup(messsageSubject, messageContent, letstalkAttachments);
            var result = letstalkService.SendMessage(letstalkModel);
            return result;
        }
    }
}