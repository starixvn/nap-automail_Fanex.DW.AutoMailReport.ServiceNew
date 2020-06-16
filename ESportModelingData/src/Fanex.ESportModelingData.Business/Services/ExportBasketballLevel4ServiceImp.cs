using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportBasketballLevel4ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService export;

        public ExportBasketballLevel4ServiceImp(ExportBaseService export)
        {
            this.export = export;
        }

        public void Execute(IGrouping<dynamic, RawModelingDataModel> level)
        {
            level.GroupBy(g => new { g.Game })
            .ToList()
            .ForEach(game =>
            {
                game.GroupBy(b => new { b.MainBetType })
                .ToList()
                .ForEach(mainBetType =>
                {
                    mainBetType.GroupBy(btg => new { btg.BetTypeGroup })
                    .ToList()
                    .ForEach(btgRow =>
                    {
                        var sportFolder = export.Replace(level.Key.Sport);
                        var gameFolder = export.Replace(game.Key.Game);
                        var mainBetTypeFolder = export.Replace(mainBetType.Key.MainBetType);
                        var bettypeGroupFolder = export.Replace(btgRow.Key.BetTypeGroup);
                        var folder = $@"{sportFolder}\{gameFolder}\{mainBetTypeFolder}\{bettypeGroupFolder}";
                        var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
                        Directory.CreateDirectory(path);

                        btgRow.GroupBy(s => new { s.Status })
                        .ToList()
                        .ForEach(status =>
                        {
                            export.ExporDimension(path, btgRow.Key.BetTypeGroup + " Group", status.Key.Status, status);
                        });
                    });
                });
            });
        }
    }
}