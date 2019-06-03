using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Fanex.BetBuilder.Business.Models;
using ICSharpCode.SharpZipLib.Zip;

namespace Fanex.BetBuilder.Business.Services
{
    public interface CsvService
    {
        string ExportCsv(List<DailyDataModel> data);
    }

    public class CsvServiceImp : CsvService
    {
        private readonly CriteriaService param;

        public CsvServiceImp(CriteriaService param)
        {
            this.param = param;
        }

        public string ExportCsv(List<DailyDataModel> data)
        {
            var csvPath = GenerateFileName();
            SaveFileCSV(data, csvPath);
            var zipPath = ZipFile(csvPath);
            Delete(csvPath);

            return zipPath;
        }

        private static void SaveFileCSV(List<DailyDataModel> data, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(data);
                }
            }
        }

        private string GenerateFileName()
        {
            var fileName = $@"Bet_Builder_Daily_Report_{param.WinlostDate:yyyyMMdd}.csv";
            var path = $@"{AppConfigService.AppConfig.FilePath}\{param.WinlostDate:yyyy}\{param.WinlostDate:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";

            return path;
        }

        private static string ZipFile(string path)
        {
            var zipFile = $@"{path.Replace(".csv", string.Empty)}.zip";
            var shortName = new FileInfo(path).Name;
            var folderPath = new DirectoryInfo(path).Parent.FullName;
            var zip = new FastZip();
            zip.Password = AppConfigService.AppConfig.Password;
            zip.CreateZip(zipFile, folderPath, false, shortName);

            return zipFile;
        }

        private static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}