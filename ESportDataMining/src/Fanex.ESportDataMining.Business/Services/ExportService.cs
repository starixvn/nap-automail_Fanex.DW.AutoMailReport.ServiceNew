using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ESportDataMining.Business.Services
{
    public interface ExportService
    {
        void Export<T>(List<T> data, string csvFileName);

        void CleanUpFiles();
    }

    public class ExportServiceImp : ExportService
    {
        private readonly CsvService csvService;
        private readonly DateTimeService dateTimeService;
        private string LocalFolder, ParentFolder;

        public ExportServiceImp(CsvService csvService, DateTimeService dateTimeService)
        {
            this.csvService = csvService;
            this.dateTimeService = dateTimeService;
        }

        public void Export<T>(List<T> data, string csvFileName)
        {
            ParentFolder = $@"{dateTimeService.ToDate():yyyy}\{dateTimeService.ToDate():MM}";
            var subFolderName = $@"{ParentFolder}\E-Sports Weekly PowerBI Data From {dateTimeService.FromDate():yyyy-MM-dd} To {dateTimeService.ToDate():yyyy-MM-dd}";
            LocalFolder = $@"{AppConfigService.AppConfig.FilePath}\{subFolderName}";

            Directory.CreateDirectory(LocalFolder);

            var fileName = $@"{LocalFolder}\{csvFileName}";
            csvService.Execute(data, fileName);
        }

        public void CleanUpFiles()
        {
            var zipFile = ZipFile();
            MoveFile(ParentFolder, zipFile);
            DeleteFolder(AppConfigService.AppConfig.FilePath);
        }

        private static void MoveFile(string folderName, string zipFile)
        {
            var path = AppConfigService.AppConfig.Path += $@"\{folderName}";
            Directory.CreateDirectory(path);

            var fileName = new FileInfo(zipFile).Name;
            var fullPath = path + @"\" + fileName;
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            File.Move(zipFile, fullPath);
        }

        private string ZipFile()
        {
            var path = LocalFolder;
            var zipFile = $"{path}.zip";
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
    }
}