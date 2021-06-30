using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface MessageService
    {
        List<string> SendMessage();

        List<string> GetReportNameNotReady();
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private List<string> foldersNotReady = new List<string>();
        private Dictionary<string, OneTimeModel> foldersReadyToSend = new Dictionary<string, OneTimeModel>();
        private List<string> reportNameNotReady = new List<string>();
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly DataService dataService;

        public MessageServiceImp(
            LetstalkClientService letstalkService,
            FileService fileService,
            MailService mailService,
            DataService dataService)
        {
            this.letstalkService = letstalkService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.dataService = dataService;
        }

        private void RefineData()
        {
            var subFolders = AppConfigService.AppConfig.Folders.Where(s => s.IsActive).ToList();
            var logs = dataService.QueryReportPreRunLog();

            foreach (var f in subFolders)
            {
                var logFolder = logs.LastOrDefault(s => s.FolderName == f.FolderName);
                if (logFolder == null)
                {
                    if (f.FolderMode == FolderMode.Daily)
                    {
                        foldersNotReady.Add(f.FolderName);
                        reportNameNotReady.Add(f.ReportName);
                    }

                    continue;
                }

                var subFolder = $@"{AppConfigService.AppConfig.Path}\{f.FolderName}\{logFolder.Date}";
                if (!Directory.Exists(subFolder))
                {
                    foldersNotReady.Add(f.FolderName);
                    reportNameNotReady.Add(f.ReportName);
                    continue;
                }

                var subFiles = Directory.EnumerateFiles(subFolder);
                if (subFiles.Count() == 0)
                {
                    foldersNotReady.Add(f.FolderName);
                    reportNameNotReady.Add(f.ReportName);
                    continue;
                }

                foreach (var file in subFiles)
                {
                    var createdDate = new FileInfo(file).LastWriteTime;
                    var now = DateTime.Now;
                    var diff = (now - createdDate).TotalSeconds;
                    if (diff > 10800)
                    {
                        foldersNotReady.Add($"{file} is invalid file. Be careful !!!, You maybe send the old file to users. Please have a check.");
                        if (!reportNameNotReady.Contains(f.ReportName))
                        {
                            reportNameNotReady.Add(f.ReportName);
                        }

                        continue;
                    }
                }

                foldersReadyToSend.Add(subFolder, logFolder);
            }
        }

        public List<string> SendMessage()
        {
            RefineData();

            var countRetry = 0;
            var foldersSentBefore = fileService.ReadFolderSent();
            var foldersHaveSent = new List<string>();

            if (foldersNotReady.Count > 0)
            {
                foldersNotReady.Insert(0, "Folders below are not ready to send to users. Please have a check!!!");
            }

            foreach (var f in foldersReadyToSend)
            {
                if (foldersSentBefore.Contains(f.Value.FolderName))
                {
                    continue;
                }

                var fileNames = Directory.GetFiles(f.Key).OrderBy(s => s).ToList(); ;
                var dictFiles = new Dictionary<int, string>();
                fileNames.ForEach(filename =>
                {
                    var index = Convert.ToInt32(new FileInfo(filename).Name.Split('_')[0]);
                    dictFiles.Add(index, filename);
                });

                var finalFiles = dictFiles.OrderBy(o => o.Key);

                foreach (var filename in finalFiles)
                {
                    var fileInfo = new FileInfo(filename.Value);
                    var isInValidFile = fileInfo.Extension.Contains(".db");
                    if (isInValidFile)
                    {
                        continue;
                    }

                    if (f.Value.MailId > 0)
                    {
                        SendMail(f.Value, filename.Value);
                        if (!foldersHaveSent.Contains(f.Value.FolderName))
                        {
                            foldersHaveSent.Add(f.Value.FolderName);
                        }
                    }

                    if (string.IsNullOrEmpty(f.Value.Group))
                    {
                        continue;
                    }

                    var letstalkModel = CreateLetstalkModel(new List<string> { filename.Value }, f.Value.TargetName, f.Value.Group);
                    var status = letstalkService.SendMessage(letstalkModel);
                    if (status && !foldersHaveSent.Contains(f.Value.FolderName))
                    {
                        foldersHaveSent.Add(f.Value.FolderName);
                    }

                    countRetry = 0;
                    while (!status && countRetry < 10)
                    {
                        status = letstalkService.SendMessage(letstalkModel);
                        countRetry++;
                    }

                    if (!status)
                    {
                        foldersNotReady.Insert(0, $"The files below can't be sent to users. Please have a check!!!");
                        foldersNotReady.Add(filename.Value);
                    }
                }
            }

            fileService.WriteFolderSent(foldersHaveSent);

            return foldersNotReady;
        }

        private void SendMail(OneTimeModel model, string filename)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(filename, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Subject = model.Subject,
                Body = fileService.LoadMailTemplate(new List<string> { new FileInfo(filename).Name }),
                Attachments = attachments,
                MailingListId = model.MailId,
            };

            mailService.SendMail(mail);
        }

        private LetstalkModel CreateLetstalkModel(List<string> fileNames, string targetName, string letstalkGroup)
        {
            var letstalkAttachments = new List<LetstalkAttachment>();
            fileNames.ForEach(filename =>
            {
                letstalkAttachments.Add(new LetstalkAttachment
                {
                    FileName = filename,
                    Bytes = FileBinaryService.ConvertFileToBinary(filename)
                });
            });

            return new LetstalkModel
            {
                TargetName = targetName,
                Group = letstalkGroup,
                LetstalkAttachments = letstalkAttachments,
                Subject = string.Empty,
                Content = string.Empty
            };
        }

        public List<string> GetReportNameNotReady()
        {
            return reportNameNotReady;
        }
    }
}