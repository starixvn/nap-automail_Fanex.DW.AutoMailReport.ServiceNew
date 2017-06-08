using System.Data;
using Fanex.Data;
using Fanex.Data.Repository;

namespace BettypeWith3rdProducts.Business.Repositories
{
    public interface DataSourceService
    {
        DataTable Query(CriteriaBase criteria);
    }

    public class DataSourceServiceImp : DataSourceService
    {
        public DataTable Query(CriteriaBase criteria)
        {
            var dataTable = new DataTable();
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                db.AssignParamValues(criteria.GetSpParams());
                var dataset = db.ExecuteDataSet();
                dataTable = dataset != null && dataset.Tables.Count > 0
                    ? dataset.Tables[0]
                    : dataTable;
            }

            return dataTable;
        }
    }
}