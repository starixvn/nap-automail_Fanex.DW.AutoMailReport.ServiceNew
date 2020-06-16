using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public class GenerateTemplateWeeklyHistoryImp : GenerateTemplate
    {
        private readonly DateTimeService dateService;

        public GenerateTemplateWeeklyHistoryImp(DateTimeService dateService)
        {
            this.dateService = dateService;
        }

        public string CustInfoPath(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $@"{ParentFolder()}\{sportName}";
        }

        public string ParentFolder()
        {
            return $@"Weekly\{dateService.DateStart.ToOADate()}";
        }

        public string ParentFolderPath(Sport sport, Period period)
        {
            return $@"{ParentFolder()}";
        }

        public string Subject(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $"{sportName} {AppConfigService.AppConfig.Period} Modelling Data From {dateService.DateStart:dd-MM-yyyy} To {dateService.DateEnd:dd-MM-yyyy}";
        }
    }
}