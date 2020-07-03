using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportESportLevel3ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportESportLevel3ServiceImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public void Execute(IGrouping<dynamic, RawModelingDataModel> level)
        {
            level.GroupBy(g => new { g.Game })
            .ToList()
            .ForEach(game =>
            {
                game.GroupBy(b => new { b.BetTypeGroup })
                .ToList()
                .ForEach(bettypeGroup =>
                {
                    var root = AppConfigService.DateStart.ToOADate();
                    var sport = exportBaseService.Replace(level.Key.Sport);
                    var folder = $@"{root}\{sport}\{exportBaseService.Replace(game.Key.Game)}\{exportBaseService.Replace(bettypeGroup.Key.BetTypeGroup)}";
                    var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
                    Directory.CreateDirectory(path);

                    bettypeGroup.GroupBy(s => new { s.Status })
                    .ToList()
                    .ForEach(status =>
                    {
                        exportBaseService.ExporDimension(path, bettypeGroup.Key.BetTypeGroup + " Group", status.Key.Status, status);
                    });
                });
            });
        }
    }
}