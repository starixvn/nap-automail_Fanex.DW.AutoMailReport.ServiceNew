using Fanex.Data.Repository;
using Fanex.Service.Core;
using Fanex.TopLicensee.Service.Criterias;
using Fanex.TopLicensee.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TopLicensee.Service.Services
{
    public interface DataService
    {
        bool IsDataAvailable();

        LastDateModel QueryLastDate();

        List<LicenseeTOModel> QueryTopLicensee(int sportType);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateService dateService;

        public DataServiceImp(IDynamicRepository repository, DateService dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public bool IsDataAvailable()
        {
            if (!AppConfigService.AppConfig.IsCheckEnoughData)
            {
                return true;
            }

            return QueryLastDate().LastDate >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());
        }

        public LastDateModel QueryLastDate()
        {
            return repository.Get<LastDateModel>(new GetDWStatusByCubeCriteria());
        }

        public List<LicenseeTOModel> QueryTopLicensee(int sportType)
        {
            var criteria = new TopLicenseeByTOCriteria { RptDate = dateService.GetRptDate(), SportType = sportType };
            var result = repository.FetchMultiple<SectionModel, LicenseeTOModel>(criteria);            
            var data = result.Item2.ToList();
            return data;
        }
    }
}