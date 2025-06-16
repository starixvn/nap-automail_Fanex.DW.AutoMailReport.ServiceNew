using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
using Fanex.MailDelivery.Core.Models;
using Fanex.Mattermost.Client;
using Fanex.Mattermost.Client.Models;
using Newtonsoft.Json;
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
        private MattermostClientService mattermostService;
        private List<string> foldersNotReady = new List<string>();
        private Dictionary<string, OneTimeModel> foldersReadyToSend = new Dictionary<string, OneTimeModel>();
        private List<string> reportNameNotReady = new List<string>();
        private readonly FileService fileService;
        private readonly MailService mailService;

        public MessageServiceImp(
            LetstalkClientService letstalkService,
            MattermostClientService mattermostService,
            FileService fileService,
            MailService mailService)
        {
            this.letstalkService = letstalkService;
            this.mattermostService = mattermostService;
            this.fileService = fileService;
            this.mailService = mailService;
        }

        private void RefineData()
        {
            var oneTimeLog = $@"{AppConfigService.AppConfig.Path}\OneTimeLog.json";
            var subFolders = AppConfigService.AppConfig.Folders.Where(s => s.IsActive).ToList();

            if (!File.Exists(oneTimeLog))
            {
                foldersNotReady.Add($"The file {oneTimeLog} can't be found.");
                foreach (var f in subFolders)
                {
                    reportNameNotReady.Add(f.ReportName);
                }

                return;
            }

            var text = File.ReadAllText(oneTimeLog);
            var logs = JsonConvert.DeserializeObject<List<OneTimeModel>>(text);

            foreach (var f in subFolders)
            {
                var logFolder =
                    logs.LastOrDefault(s => s.FolderName == f.FolderName && !s.TargetName.Equals("Mattermost"));
                RefineDataToSend("Letstalk", logFolder, f);

                logFolder = logs.LastOrDefault(s => s.FolderName == f.FolderName && s.TargetName.Equals("Mattermost"));
                if (logFolder != null)
                {
                    RefineDataToSend("Mattermost", logFolder, f);
                }
            }
        }

        private void RefineDataToSend(string sentBy, OneTimeModel logFolder, FolderRegisterModel f)
        {
            if (logFolder == null)
            {
                if (f.FolderMode == FolderMode.Daily)
                {
                    foldersNotReady.Add(f.FolderName);
                    reportNameNotReady.Add(f.ReportName);
                }

                return;
            }

            var subFolder = $@"{AppConfigService.AppConfig.Path}\{f.FolderName}\{logFolder.Date}";
            var subFiles = Directory.EnumerateFiles(subFolder).ToList();
            if (!subFiles.Any())
            {
                foldersNotReady.Add(f.FolderName);
                reportNameNotReady.Add(f.ReportName);
                return;
            }

            foreach (var file in subFiles)
            {
                var createdDate = new FileInfo(file).LastWriteTime;
                var now = DateTime.Now;
                var diff = (now - createdDate).TotalSeconds;
                if (diff > 10800)
                {
                    foldersNotReady.Add(
                        $"{file} is invalid file. Be careful !!!, You maybe send the old file to users. Please have a check.");
                    if (!reportNameNotReady.Contains(f.ReportName))
                    {
                        reportNameNotReady.Add(f.ReportName);
                    }
                }
            }

            foldersReadyToSend.Add($"{sentBy}|{subFolder}", logFolder);
        }

        public List<string> SendMessage()
        {
            RefineData();

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

                var fileNames = Directory.GetFiles(f.Key.Split('|')[1]).ToList();
                foreach (var filename in fileNames)
                {
                    var fileInfo = new FileInfo(filename);
                    var isInValidFile = fileInfo.Extension.Contains(".db");
                    if (isInValidFile)
                    {
                        continue;
                    }

                    if (f.Value.MailId > 0)
                    {
                        SendMail(f.Value, filename);
                        if (!foldersHaveSent.Contains(f.Value.FolderName))
                        {
                            foldersHaveSent.Add(f.Value.FolderName);
                        }
                    }

                    if (string.IsNullOrEmpty(f.Value.Group))
                    {
                        continue;
                    }

                    if (f.Value.TargetName.Equals("Mattermost"))
                    {
                        var messageRequest = new MessageRequest
                        {
                            ChannelId = f.Value.Group,
                            Files = new List<FileModel>
                            {
                                new FileModel
                                {
                                    FileName = fileInfo.Name,
                                    FileBytes = File.ReadAllBytes(filename),
                                    MediaType = fileInfo.Extension.Contains(".xlsx") ? MediaType.Excel : MediaType.Image
                                }
                            },
                        };
                        var result = mattermostService.Send(messageRequest);
                        System.Threading.Thread.Sleep(2000); // To avoid rate limit
                    }
                    else
                    {
                        SendLetstalk(filename, f, foldersHaveSent);
                    }
                }
            }

            fileService.WriteFolderSent(foldersHaveSent);

            return foldersNotReady;
        }

        private void SendLetstalk(string filename, KeyValuePair<string, OneTimeModel> f, List<string> foldersHaveSent)
        {
            var countRetry = 0;
            var letstalkModel = CreateLetstalkModel(new List<string> { filename }, f.Value.TargetName,
                f.Value.Group);
            var status = letstalkService.SendMessage(letstalkModel);
            if (status && !foldersHaveSent.Contains(f.Value.FolderName))
            {
                foldersHaveSent.Add(f.Value.FolderName);
            }

            while (!status && countRetry < 10)
            {
                status = letstalkService.SendMessage(letstalkModel);
                countRetry++;
            }

            if (!status)
            {
                foldersNotReady.Insert(0,
                    $"The files below can't be sent to users. Please have a check!!!");
                foldersNotReady.Add(filename);
            }
        }

        private void SendMail(OneTimeModel model, string filename)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(filename, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Title = "Auto Generated Report Administrator",
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