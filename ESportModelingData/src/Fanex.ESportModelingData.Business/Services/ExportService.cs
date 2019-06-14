using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fanex.ESportModelingData.Business.Models;
using ICSharpCode.SharpZipLib.Zip;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportService
    {
        string Execute(DateTime dateStart, DateTime dateEnd, List<RawModelingDataModel> rawData);
    }

    public class ExportServiceImp : ExportService
    {
        private readonly ExportLevelFactory exportLevelFactory;

        public ExportServiceImp(ExportLevelFactory exportLevelFactory)
        {
            this.exportLevelFactory = exportLevelFactory;            
        }

        public string Execute(DateTime dateStart, DateTime dateEnd, List<RawModelingDataModel> rawData)
        {
            var parentFolder = $@"{dateEnd:yyyy}\{dateEnd:MM}";
            var folderName = $@"{parentFolder}\E-Sports Weekly Modeling Data From {dateStart:dd-MM-yyyy} To {dateEnd:dd-MM-yyyy}";
            var originalPath = AppConfigService.AppConfig.FilePath;
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

            var result = ZipFile();
            Delete();

            AppConfigService.AppConfig.FilePath = originalPath;

            return result;
        }

        private static string ZipFile()
        {
            var path = AppConfigService.AppConfig.FilePath;
            var zipFile = $"{AppConfigService.AppConfig.FilePath}.zip";
            var zip = new FastZip();
            zip.Password = AppConfigService.AppConfig.Password;
            zip.CreateZip(zipFile, path, true, string.Empty);

            return zipFile;
        }

        private static void Delete()
        {
            var path = AppConfigService.AppConfig.FilePath;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}