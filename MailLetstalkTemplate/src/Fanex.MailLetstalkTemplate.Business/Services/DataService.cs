using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.MailLetstalkTemplate.Business.Criterias;
using Fanex.MailLetstalkTemplate.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface DataService
    {
        DataSet QueryData(string spName);

        List<MailModel> QueryMail(DateTime rptDate);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateRangeServiceProfier dateRangeServiceProfier;

        public DataServiceImp(DateRangeServiceProfier dateRangeServiceProfier, IDynamicRepository repository)
        {
            this.dateRangeServiceProfier = dateRangeServiceProfier;
            this.repository = repository;
        }

        public DataSet QueryData(string spName)
        {
            var date = dateRangeServiceProfier.Date(AppConfigService.Config.DateRange);
            ObjectDb db = new ObjectDb(spName);
            var param = new Dictionary<string, DateTime>
            {
                { "FromDate", date.FromDate },
                { "ToDate", date.ToDate},
            };

            db.AssignParamValues(param);

            return db.ExecuteDataSet();            
        }

        public List<MailModel> QueryMail(DateTime rptDate)
        {
            var criteria = new CheckMailStatusCriteria { FromDate = rptDate, ToDate = rptDate };
            return repository.Fetch<MailModel>(criteria).ToList();
        }
    }
}