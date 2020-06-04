using Fanex.ESportModelingData.Business.Models;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportService
    {
        string Execute(DateTime dateStart, DateTime dateEnd, List<RawModelingDataModel> rawData);

        void ExportCustInfo(DateTime dateStart, DateTime dateEnd, List<CustInfo> rawData);
    }

    public class ExportServiceImp : ExportService
    {
        private readonly ExportLevelFactory exportLevelFactory;
        private readonly CsvService csvService;
        private readonly GenerateTemplateProvider templateService;

        public ExportServiceImp(ExportLevelFactory exportLevelFactory, CsvService csvService, GenerateTemplateProvider templateService)
        {
            this.exportLevelFactory = exportLevelFactory;
            this.csvService = csvService;
            this.templateService = templateService;
        }

        public string Execute(DateTime dateStart, DateTime dateEnd, List<RawModelingDataModel> rawData)
        {
            var template = templateService.Execute(AppConfigService.AppConfig.Period, dateStart);
            var parentFolder = template.ParentFolder();
            var folderName = template.ParentFolderPath(AppConfigService.AppConfig.Sport, AppConfigService.AppConfig.Period);
            var originalFilePath = AppConfigService.AppConfig.FilePath;
            var originalPath = AppConfigService.AppConfig.Path;
            AppConfigService.AppConfig.FilePath += $@"\{folderName}";
            var levelInstance = exportLevelFactory.Execute();
            rawData
                .OrderBy(o => o.Level)
                .GroupBy(g => new { g.Level, g.Sport })
                .ToList()
                .ForEach(level =>
                {
                    levelInstance[level.Key.Level].Execute(level);
                });

            var zipFile = ZipFile();
            if (AppConfigService.AppConfig.Period != Period.WeeklyHistoricalData && AppConfigService.AppConfig.Period != Period.DailyHistoricalData)
            {
                MoveFile(parentFolder, zipFile);
                DeleteFolder(originalFilePath);
            }

            AppConfigService.AppConfig.FilePath = originalFilePath;
            AppConfigService.AppConfig.Path = originalPath;

            return zipFile;
        }

        private static void MoveFile(string folderName, string zipFile)
        {
            AppConfigService.AppConfig.Path += $@"\{folderName}";
            Directory.CreateDirectory(AppConfigService.AppConfig.Path);

            var fileName = new FileInfo(zipFile).Name;
            var fullPath = AppConfigService.AppConfig.Path + @"\" + fileName;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            File.Move(zipFile, fullPath);
        }

        private static string ZipFile()
        {
            var path = AppConfigService.AppConfig.FilePath;
            if (AppConfigService.AppConfig.Period == Period.WeeklyHistoricalData || AppConfigService.AppConfig.Period == Period.DailyHistoricalData)
            {
                return path;
            }

            var zipFile = $"{AppConfigService.AppConfig.FilePath}.zip";
            var zip = new FastZip();
            zip.Password = AppConfigService.AppConfig.Password;
            zip.CreateZip(zipFile, path, true, string.Empty);

            return zipFile;
        }

        private static void DeleteFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void ExportCustInfo(DateTime dateStart, DateTime dateEnd, List<CustInfo> rawData)
        {
            var template = templateService.Execute(AppConfigService.AppConfig.Period, dateStart);
            var folderName = template.ParentFolderPath(AppConfigService.AppConfig.Sport, AppConfigService.AppConfig.Period);

            var path = $@"{AppConfigService.AppConfig.FilePath}\{folderName}";
            Directory.CreateDirectory(path);
            var fileName = $@"{path}\E-Sports - Customer Infomation.csv";
            csvService.Execute(rawData, fileName);
        }
    }
}