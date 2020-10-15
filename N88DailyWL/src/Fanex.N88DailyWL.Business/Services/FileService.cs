using Fanex.N88DailyWL.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface FileService
    {
        void CopyFiles(List<string> fileNames, string letstalkGroup);
    }

    public class FileServiceImp : FileService
    {
        private readonly DateService dateService;

        public FileServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        private void LogOneTime(DateTime date, string letstalkGroup)
        {
            var fileInfo = new FileInfo($"{AppConfigService.AppConfig.InternalPath}-{letstalkGroup}");
            var path = fileInfo.DirectoryName;
            var fileName = $@"{path}\OneTimeLog.json";
            var logs = new List<LogInternalOneTimeModel>();
            if (File.Exists(fileName))
            {
                var text = File.ReadAllText(fileName);
                logs = JsonConvert.DeserializeObject<List<LogInternalOneTimeModel>>(text);
            }

            logs.Add(new LogInternalOneTimeModel
            {
                Date = $"{date:yyyyMMdd}",
                FolderName = fileInfo.Name,
                GenerateTime = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt}",
                TargetName = AppConfigService.AppConfig.TargetName,
                Group = letstalkGroup
            });

            var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public void CopyFiles(List<string> fileNames, string letstalkGroup)
        {
            var date = dateService.ToDate;
            var path = $@"{AppConfigService.AppConfig.InternalPath}-{letstalkGroup}\{date:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var index = 1;
            fileNames.ForEach(fileName =>
            {
                var sortName = $"{index}_{new FileInfo(fileName).Name}";
                var destinationPath = $@"{path}\{sortName}";
                File.Copy(fileName, destinationPath, true);
                index++;
            });

            LogOneTime(date, letstalkGroup);
        }
    }
}