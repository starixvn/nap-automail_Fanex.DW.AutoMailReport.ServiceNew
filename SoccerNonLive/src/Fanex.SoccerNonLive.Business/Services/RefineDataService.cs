using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface RefineDataService
    {
        DataTable Execute(DataTable rawData);
    }

    public class RefineDataServiceImp : RefineDataService
    {
        public DataTable Execute(DataTable rawData)
        {
            throw new NotImplementedException();
        }
    }
}