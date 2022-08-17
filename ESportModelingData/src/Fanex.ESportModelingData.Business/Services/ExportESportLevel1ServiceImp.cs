using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportESportLevel1ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportESportLevel1ServiceImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public void Execute(IGrouping<dynamic, RawModelingDataModel> level)
        {
            var root = AppConfigService.DateStart.ToOADate();
            var sport = exportBaseService.Replace(level.Key.Sport);
            var path = $@"{AppConfigService.AppConfig.FilePath}\{root}\{sport}";
            Directory.CreateDirectory(path.Trim());

            level.GroupBy(s => new { s.Status })
            .ToList()
            .ForEach(status =>
            {
                exportBaseService.ExporDimension(path, level.Key.Sport, status.Key.Status, status);
            });
        }
    }
}