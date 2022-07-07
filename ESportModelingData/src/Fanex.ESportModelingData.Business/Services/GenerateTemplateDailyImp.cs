using Fanex.ESportModelingData.Business.Models;
using System;

namespace Fanex.ESportModelingData.Business.Services
{
    public class GenerateTemplateDailyImp : GenerateTemplate
    {
        private readonly DateTimeService dateService;

        public GenerateTemplateDailyImp(DateTimeService dateService)
        {
            this.dateService = dateService;
        }

        public string CustInfoPath(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            var root = dateService.DateStart.ToOADate();
            return $@"{ParentFolder()}\{sportName} {AppConfigService.AppConfig.Period} For {dateService.DateEnd:dd}\{root}\{sportName}";
        }

        public string ParentFolder()
        {
            return $@"{dateService.DateEnd:yyyy}\{dateService.DateEnd:MM}";
        }

        public string ParentFolderPath(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $@"{ParentFolder()}\{sportName} {AppConfigService.AppConfig.Period} For {dateService.DateEnd:dd}";
        }

        public string Subject(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $"{sportName} {AppConfigService.AppConfig.Period} Modelling Data For {dateService.DateEnd:dd-MM-yyyy}";
        }
    }
}