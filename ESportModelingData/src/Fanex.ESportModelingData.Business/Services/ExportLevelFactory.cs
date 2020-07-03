using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportLevelFactory
    {
        Dictionary<int, ExportLevelService> ExecuteESports();

        Dictionary<int, ExportLevelService> ExecuteBasketball();
    }

    public class ExportLevelFactoryImp : ExportLevelFactory
    {
        private readonly ExportBaseService exportBaseService;

        public ExportLevelFactoryImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public Dictionary<int, ExportLevelService> ExecuteBasketball()
        {
            const int Level1 = 1;
            const int Level2 = 2;
            const int Level3 = 3;
            const int Level4 = 4;
            const int Level5 = 5;

            return new Dictionary<int, ExportLevelService>
            {
                { Level1, new ExportBasketballLevel1ServiceImp(exportBaseService) },
                { Level2, new ExportBasketballLevel2ServiceImp(exportBaseService) },
                { Level3, new ExportBasketballLevel3ServiceImp(exportBaseService) },
                { Level4, new ExportBasketballLevel4ServiceImp(exportBaseService) },
                { Level5, new ExportBasketballLevel5ServiceImp(exportBaseService) }
            };
        }

        public Dictionary<int, ExportLevelService> ExecuteESports()
        {
            const int Level1 = 1;
            const int Level2 = 2;
            const int Level3 = 3;
            const int Level4 = 4;

            return new Dictionary<int, ExportLevelService>
            {
                { Level1, new ExportESportLevel1ServiceImp(exportBaseService) },
                { Level2, new ExportESportLevel2ServiceImp(exportBaseService) },
                { Level3, new ExportESportLevel3ServiceImp(exportBaseService) },
                { Level4, new ExportESportLevel4ServiceImp(exportBaseService) }
            };
        }
    }
}