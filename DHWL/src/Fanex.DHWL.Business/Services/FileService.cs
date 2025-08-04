using Fanex.DHWL.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.DHWL.Business.Services
{
    public interface FileService
    {
        void MoveFiles(string source, string destination);

        void CopyFiles(LetstalkGroup letstalkGroup, List<string> fileNames);
    }

    public class FileServiceImp : FileService
    {
        private readonly DateTimeServiceProvider dateTimeService;

        public FileServiceImp(DateTimeServiceProvider dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public void MoveFiles(string source, string destination)
        {
            DirectoryMove(source, destination, true);
            DeleteEmptyDirectoryOlderThanOneDay(source);
        }

        private void DirectoryMove(string source, string destination, bool includeSubDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found or need a permission to access on this: " +
                    source);
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var files = directoryInfo.GetFiles("*.*");
            foreach (var file in files)
            {
                var destinationFile = Path.Combine(destination, file.Name);

                SafeMoveFile(file, destinationFile);
            }

            if (includeSubDirectory)
            {
                var subSourceDirectories = directoryInfo.GetDirectories();
                foreach (var subSourceDirectory in subSourceDirectories)
                {
                    string subDestinationDirectory = Path.Combine(destination, subSourceDirectory.Name);
                    DirectoryMove(subSourceDirectory.FullName, subDestinationDirectory, includeSubDirectory);
                }
            }
        }

        private static void SafeMoveFile(FileInfo file, string destinationFile)
        {
            if (!File.Exists(destinationFile)
                || file.LastWriteTime > (new FileInfo(destinationFile)).LastWriteTime)
            {
                file.CopyTo(destinationFile, true);
            }

            try
            {
                var ageOfFile = (DateTime.Now - file.LastWriteTime).TotalHours;
                if (ageOfFile >= 25)
                {
                    file.Delete();
                }
            }
            catch (IOException)
            {
                // File is in use
            }
        }

        private static void DeleteEmptyDirectoryOlderThanOneDay(string location)
        {
            foreach (var directory in Directory.GetDirectories(location))
            {
                DeleteEmptyDirectoryOlderThanOneDay(directory);
                var createdDate = Directory.GetCreationTime(directory);
                var isOld = createdDate <= DateTime.Now.AddDays(-1);
                if (isOld
                    && Directory.GetFiles(directory).Length == 0
                    && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        private void LogOneTime(LetstalkGroup letstalkGroup, DateTime date)
        {
            var fileInfo = new FileInfo(letstalkGroup.InternalPath);
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
                TargetName = AppConfigService.Config.TargetName,
                Group = letstalkGroup.Group,
                MailId = letstalkGroup.MailId,
                Subject = letstalkGroup.MailId > 0
                    ? $"Automail Report - House Net Income - {date.AddDays(1):yyyy/MM/dd}"
                    : string.Empty
            });

            var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public void CopyFiles(LetstalkGroup letstalkGroup, List<string> fileNames)
        {
            var date = dateTimeService.DateTime.ToDate;
            var path = $@"{letstalkGroup.InternalPath}\{date:yyyyMMdd}";
            Directory.CreateDirectory(path);
            var index = 1;
            fileNames.ForEach(fileName =>
            {
                var sortName = $"{index}_{new FileInfo(fileName).Name}";
                var destinationPath = $@"{path}\{sortName}";
                File.Copy(fileName, destinationPath, true);
                index++;
            });

            LogOneTime(letstalkGroup, date);
        }
    }
}