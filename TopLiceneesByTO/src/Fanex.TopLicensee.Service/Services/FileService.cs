using Fanex.TopLicensee.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.TopLicensee.Service.Services
{
    public interface FileService
    {
        void CopyFiles(bool isLicBySport, List<string> fileNames);
    }

    public class FileServiceImp : FileService
    {
        private readonly DateService dateService;

        public FileServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        private void LogOneTime(bool isLicBySport, DateTime date)
        {
            var fileInfo = isLicBySport
                ? new FileInfo(AppConfigService.AppConfig.InternalPathLicBySport)
                : new FileInfo(AppConfigService.AppConfig.InternalPath);
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
                Group = isLicBySport ? AppConfigService.AppConfig.GroupLicBySport : AppConfigService.AppConfig.Group
            });

            var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public void CopyFiles(bool isLicBySport, List<string> fileNames)
        {
            var date = dateService.ToDate;
            var path = isLicBySport
                ? $@"{AppConfigService.AppConfig.InternalPathLicBySport}\{date:yyyyMMdd}"
                : $@"{AppConfigService.AppConfig.InternalPath}\{date:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var index = 1;
            fileNames.ForEach(fileName =>
            {
                var sortName = $"{index}_{new FileInfo(fileName).Name}";
                var destinationPath = $@"{path}\{sortName}";
                File.Copy(fileName, destinationPath, true);
                index++;
            });

            LogOneTime(isLicBySport, date);
        }
    }
}