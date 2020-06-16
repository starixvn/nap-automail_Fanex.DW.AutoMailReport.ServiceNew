using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public class GenerateTemplateWeeklymp : GenerateTemplate
    {
        private readonly DateTimeService dateService;

        public GenerateTemplateWeeklymp(DateTimeService dateService)
        {
            this.dateService = dateService;
        }

        public string CustInfoPath(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $@"{ParentFolder()}\{sportName} {AppConfigService.AppConfig.Period} Modelling Data From {dateService.DateStart:dd-MM-yyyy} To {dateService.DateEnd:dd-MM-yyyy}\{sportName}";
        }

        public string ParentFolder()
        {
            return $@"{dateService.DateEnd:yyyy}\{dateService.DateEnd:MM}";
        }

        public string ParentFolderPath(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $@"{ParentFolder()}\{sportName} {AppConfigService.AppConfig.Period} Modelling Data From {dateService.DateStart:dd-MM-yyyy} To {dateService.DateEnd:dd-MM-yyyy}";
        }

        public string Subject(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $"{sportName} {AppConfigService.AppConfig.Period} Modelling Data From {dateService.DateStart:dd-MM-yyyy} To {dateService.DateEnd:dd-MM-yyyy}";
        }
    }
}