using System.Linq;
using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportLevelService
    {
        void Execute(IGrouping<dynamic, RawModelingDataModel> level);
    }
}