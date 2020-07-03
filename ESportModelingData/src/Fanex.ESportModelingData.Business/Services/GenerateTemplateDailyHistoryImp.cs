using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public class GenerateTemplateDailyHistoryImp : GenerateTemplate
    {
        private readonly DateTimeService dateService;

        public GenerateTemplateDailyHistoryImp(DateTimeService dateService)
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
            return $@"Daily\{dateService.DateEnd.ToOADate()}";
        }

        public string ParentFolderPath(Sport sport, Period period)
        {
            return ParentFolder();
        }

        public string Subject(Sport sport, Period period)
        {
            var sportName = GenerateName.SportNames[sport];
            return $"{sportName} {AppConfigService.AppConfig.Period} Modelling Data For {dateService.DateEnd:dd-MM-yyyy}";
        }
    }
}