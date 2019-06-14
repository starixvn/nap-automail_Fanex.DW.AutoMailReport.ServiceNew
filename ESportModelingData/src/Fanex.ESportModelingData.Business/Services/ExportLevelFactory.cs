using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportLevelFactory
    {
        Dictionary<int, ExportLevelService> Execute();
    }

    public class ExportLevelFactoryImp : ExportLevelFactory
    {
        private readonly ExportBaseService exportBaseService;

        public ExportLevelFactoryImp(ExportBaseService exportBaseService)
        {
            this.exportBaseService = exportBaseService;
        }

        public Dictionary<int, ExportLevelService> Execute()
        {
            const int Level1 = 1;
            const int Level2 = 2;
            const int Level3 = 3;
            const int Level4 = 4;

            return new Dictionary<int, ExportLevelService>
            {
                { Level1, new ExportLevel1ServiceImp(exportBaseService) },
                { Level2, new ExportLevel2ServiceImp(exportBaseService) },
                { Level3, new ExportLevel3ServiceImp(exportBaseService) },
                { Level4, new ExportLevel3ServiceImp(exportBaseService) }
            };
        }
    }
}