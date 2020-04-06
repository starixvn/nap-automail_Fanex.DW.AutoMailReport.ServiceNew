using Fanex.TopLicensee.Service.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Fanex.TopLicensee.Service.Services
{
    public interface ExportVirtualSoccerLicensee
    {
        string Export(DataTable dataTable, LeagueGroup leagueGroup, SportType sportType);
    }

    public class ExportVirtualSoccerLicenseeImp : ExportVirtualSoccerLicensee
    {
        private ExcelPackage excelPackage;
        private readonly DateService dateService;

        public ExportVirtualSoccerLicenseeImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public string Export(DataTable dataTable, LeagueGroup leagueGroup, SportType sportType)
        {
            var sport = new Dictionary<SportType, string>
            {
                { SportType.VirtualFIFA, "Saba Soccer" },
                { SportType.VirtualNBA, "Saba Basketball" },
            };

            var titleSport = new Dictionary<LeagueGroup, string>
            {
                { LeagueGroup.VirtualLeague, sport[sportType] },
                { LeagueGroup.Esoccer, "ESOCCER " },
            };

            excelPackage = new ExcelPackage(new FileInfo("Templates/TopLicensee-VirtualSoccer-Licensee.xlsx"));
            var template = excelPackage.Workbook.Worksheets["Template"];
            template.Cells["B2"].Value = titleSport[leagueGroup];
            template.Cells["D2"].Value = $"{dateService.ToDate:yyyyMMdd}";
            template.Cells["C4"].LoadFromDataTable(dataTable, false);

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