using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.LetstalkService.Core.Models;
using Fanex.LetstalkService.Core.Services;
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
    }

    public class MessageServiceImp : MessageService
    {
        private readonly LetstalkClientService letstalkService;
        private List<string> foldersNotReady = new List<string>();
        private Dictionary<string, OneTimeModel> foldersReadyToSend = new Dictionary<string, OneTimeModel>();

        public MessageServiceImp(LetstalkClientService letstalkService)
        {
            this.letstalkService = letstalkService;
        }

        private void RefineData()
        {
            var oneTimeLog = $@"{AppConfigService.AppConfig.Path}\OneTimeLog.json";
            if (!File.Exists(oneTimeLog))
            {
                return;
            }
            var text = File.ReadAllText(oneTimeLog);
            var logs = JsonConvert.DeserializeObject<List<OneTimeModel>>(text);

            var subFolders = AppConfigService.AppConfig.Folders.Where(s => s.IsActive).ToList();
            foreach (var f in subFolders)
            {
                var logFolder = logs.LastOrDefault(s => s.FolderName == f.FolderName);
                if (logFolder == null)
                {
                    if (f.FolderMode == FolderMode.Daily)
                    {
                        foldersNotReady.Add(f.FolderName);
                    }

                    continue;
                }

                var subFolder = $@"{AppConfigService.AppConfig.Path}\{f.FolderName}\{logFolder.Date}";
                var subFiles = Directory.EnumerateFiles(subFolder);
                if (subFiles.Count() == 0)
                {
                    foldersNotReady.Add(f.FolderName);
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
                        continue;
                    }
                }

                foldersReadyToSend.Add(subFolder, logFolder);
            }
        }

        public List<string> SendMessage()
        {
            var countRetry = 0;
            RefineData();
            if (foldersNotReady.Count > 0)
            {
                foldersNotReady.Insert(0, "Folders below are not ready to send to users. Please have a check!!!");
                return foldersNotReady;
            }

            foreach (var f in foldersReadyToSend)
            {
                var fileNames = Directory.GetFiles(f.Key).ToList();
                fileNames.ForEach(filename =>
                {
                    var letstalkModel = CreateLetstalkModel(new List<string> { filename }, f.Value.TargetName, f.Value.Group);
                    var status = letstalkService.SendMessage(letstalkModel);
                    countRetry = 0;
                    while (!status && countRetry < 10)
                    {
                        status = letstalkService.SendMessage(letstalkModel);
                        countRetry++;
                    }

                    if (!status)
                    {
                        foldersNotReady.Insert(0, $"The files below can't be sent to users. Please have a check!!!");
                        foldersNotReady.Add(filename);
                    }
                });
            }

            return foldersNotReady;
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
    }
}