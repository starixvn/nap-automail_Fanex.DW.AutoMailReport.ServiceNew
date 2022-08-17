using Fanex.ESportModelingData.Business.Models;
using System.IO;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public class ExportBasketballLevel3ServiceImp : ExportLevelService
    {
        private readonly ExportBaseService exportBaseService;

        public ExportBasketballLevel3ServiceImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
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
                    var root = AppConfigService.DateStart.ToOADate();
                    var sport = exportBaseService.Replace(level.Key.Sport);
                    var folder = $@"{root}\{sport}\{exportBaseService.Replace(game.Key.Game)}\{exportBaseService.Replace(mainBetType.Key.MainBetType)}";
                    var path = $@"{AppConfigService.AppConfig.FilePath}\{folder}";
                    Directory.CreateDirectory(path.Trim());

                    mainBetType.GroupBy(s => new { s.Status })
                    .ToList()
                    .ForEach(status =>
                    {
                        exportBaseService.ExporDimension(path, mainBetType.Key.MainBetType + " Group", status.Key.Status, status);
                    });
                });
            });
        }
    }
}