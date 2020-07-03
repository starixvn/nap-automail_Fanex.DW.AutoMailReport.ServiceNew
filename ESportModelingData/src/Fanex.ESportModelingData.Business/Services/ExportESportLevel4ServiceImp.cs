using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportESportLevel4ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportESportLevel4ServiceImp(ExportBaseService exportBaseService)
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
                    bettypeGroup.GroupBy(bt => new { bt.BetType })
                    .ToList()
                    .ForEach(bettype =>
                    {
                        var root = AppConfigService.DateStart.ToOADate();
                        var sport = exportBaseService.Replace(level.Key.Sport);
                        var gameName = exportBaseService.Replace(game.Key.Game);
                        var bettypeGroupName = exportBaseService.Replace(bettypeGroup.Key.BetTypeGroup);
                        var bettypeName = exportBaseService.Replace(bettype.Key.BetType);

                        var folder = $@"{root}\{sport}\{gameName}\{bettypeGroupName}\{bettypeName}";
                        var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
                        Directory.CreateDirectory(path);

                        bettype.GroupBy(s => new { s.Status })
                        .ToList()
                        .ForEach(status =>
                        {
                            exportBaseService.ExporDimension(path, bettype.Key.BetType, status.Key.Status, status);
                        });
                    });
                });
            });
        }
    }
}