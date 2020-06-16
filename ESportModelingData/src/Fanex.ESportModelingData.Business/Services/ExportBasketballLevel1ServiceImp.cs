using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportBasketballLevel1ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportBasketballLevel1ServiceImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public void Execute(IGrouping<dynamic, RawModelingDataModel> level)
        {
            var folder = exportBaseService.Replace(level.Key.Sport);
            var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
            Directory.CreateDirectory(path);

            level.GroupBy(s => new { s.Status })
            .ToList()
            .ForEach(status =>
            {
                exportBaseService.ExporDimension(path, level.Key.Sport, status.Key.Status, status);
            });
        }
    }
}