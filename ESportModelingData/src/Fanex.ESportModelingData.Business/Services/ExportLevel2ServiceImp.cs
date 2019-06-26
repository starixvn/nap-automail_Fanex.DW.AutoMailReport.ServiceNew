using System.IO;
using System.Linq;
using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportLevel2ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportLevel2ServiceImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public void Execute(IGrouping<dynamic, RawModelingDataModel> level)
        {
            level.GroupBy(g => new { g.Game })
            .ToList()
            .ForEach(game =>
            {
                var folder = $@"{exportBaseService.Replace(level.Key.Sport)}\{exportBaseService.Replace(game.Key.Game)}";
                var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
                Directory.CreateDirectory(path);

                game.GroupBy(s => new { s.Status })
                .ToList()
                .ForEach(status =>
                {
                    exportBaseService.ExporDimension(path, game.Key.Game, status.Key.Status, status);
                });
            });
        }
    }
}