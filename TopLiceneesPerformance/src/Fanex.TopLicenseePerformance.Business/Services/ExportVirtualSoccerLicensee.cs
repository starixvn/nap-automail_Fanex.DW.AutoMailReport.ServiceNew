using Fanex.TopLicensee.Service.Models;
using Fanex.TopLicenseePerformance.Business.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExportVirtualSoccerLicensee
    {
        string Export(List<TopLeagueGroupSportLicenseePerfModel> data, LeagueGroup leagueGroup, SportType sportType);

        int LastRowIndex { get; set; }
    }

    public class ExportVirtualSoccerLicenseeImp : ExportVirtualSoccerLicensee
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExportVirtualSoccerLicenseeImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public int LastRowIndex { get; set; }

        public string Export(List<TopLeagueGroupSportLicenseePerfModel> data, LeagueGroup leagueGroup, SportType sportType)
        {
            var sport = new Dictionary<SportType, string>
            {
                { SportType.Soccer, "Saba Soccer" },
                { SportType.Basketball, "Saba Basketball" },
            };

            var titleSport = new Dictionary<LeagueGroup, string>
            {
                { LeagueGroup.VL, sport[sportType] },
                { LeagueGroup.Esoccer, "ESOCCER " },
                { LeagueGroup.SabaSoccerPinGoal, "Saba Soc PGoal " },
            };

            excelPackage = new ExcelPackage(new FileInfo("Templates/TopLicensee-VirtualSoccer-Licensee.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Template"];
            template.Cells["B2"].Value = titleSport[leagueGroup];
            template.Cells["E2"].Value = $"{dateService.ToDate:yyyyMMdd}";
            template.Cells["B4"].LoadFromCollection(data, false);

            var currentIndex = 4;
            var index = 1;
            var cnt = 0;
            data.GroupBy(g => new { g.LicGroup })
                .ToList()
                .ForEach(group =>
                {
                    cnt = group.Count();
                    template.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Merge = true;
                    template.Cells[currentIndex, 2, currentIndex + cnt - 1, 2].Value = index;
                    template.Cells[currentIndex, 3, currentIndex + cnt - 1, 3].Merge = true;

                    for (var columnIndex = 5; columnIndex <= 8; columnIndex++)
                    {
                        template.Cells[currentIndex, columnIndex, currentIndex + cnt - 1, columnIndex].Merge = true;
                    }

                    currentIndex += cnt;
                    index++;
                });

            LastRowIndex = currentIndex - cnt;
            var maxRowIndex = LastRowIndex;
            var startColIndex = 2;

            var value = template.Cells[maxRowIndex, startColIndex + 1].Value;
            template.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Merge = true;
            template.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Value = value;
            template.Cells[maxRowIndex, startColIndex, maxRowIndex, startColIndex + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            return SaveCreatedFile(leagueGroup, sportType);
        }

        private string SaveCreatedFile(LeagueGroup leagueGroup, SportType sportType)
        {
            var rptDate = dateService.ToDate;
            var fileName = $"VS_Licensee_{leagueGroup}_{sportType}_For_{rptDate:yyyyMMdd}.xlsx";
            var path = $@"{AppConfigService.AppConfig.Path}\{rptDate:yyyy}\{rptDate:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{fileName}";
            excelPackage.SaveAs(new FileInfo(path), AppConfigService.AppConfig.Password);

            return path;
        }
    }
}