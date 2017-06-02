using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface ExcelService
    {
        void Execute();
    }

    public class ExcelServiceImp : ExcelService
    {
        private readonly ExcelPackage excelPackage;
        private readonly ApplicationConfigurationService appConfig;
        private readonly DataService dataService;

        public ExcelServiceImp(ExcelPackage excelPackage, ApplicationConfigurationService appConfig, DataService dataService)
        {
            this.excelPackage = excelPackage;
            this.appConfig = appConfig;
            this.dataService = dataService;

        }

        public void Execute()
        {
            var data = dataService.Execute().FirstOrDefault();
        }
    }
}
