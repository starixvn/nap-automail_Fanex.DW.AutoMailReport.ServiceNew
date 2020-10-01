using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.TopLicensee.Service.Criterias;
using Fanex.TopLicensee.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.TopLicensee.Service.Services
{
    public interface DataService
    {
        bool IsDataAvailable();

        TaskstatusModel QueryDWStatus();

        List<LicenseeTOModel> QueryTopLicensee(int sportType);

        Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> QueryDailyTopLicenseeMargin(bool isDaily);

        DataTable QueryVirtualSoccer(LeagueGroup leagueGroup, SportType sportType, string spName);
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

            return QueryDWStatus().DataIsReady;
        }

        public TaskstatusModel QueryDWStatus()
        {
            return repository.Get<TaskstatusModel>(new GetDWStatusCriteria());
        }

        public List<LicenseeTOModel> QueryTopLicensee(int sportType)
        {
            var criteria = new TopLicenseeByTOCriteria { RptDate = dateService.ToDate, SportType = sportType };
            var result = repository.FetchMultiple<SectionModel, LicenseeTOModel>(criteria);
            var data = result.Item2.ToList();
            return data;
        }

        public Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>> QueryDailyTopLicenseeMargin(bool isDaily)
        {
            var criteria = new TopLicenseeMarginCriteria
            {
                FromDate = isDaily ? dateService.ToDate : dateService.FromDate,
                ToDate = dateService.ToDate
            };

            var result = repository.FetchMultiple<TopLicenseeMarginModel, TopLicenseeMarginModel>(criteria);

            return new Tuple<List<TopLicenseeMarginModel>, List<TopLicenseeMarginModel>>(result.Item1.ToList(), result.Item2.ToList());
        }

        public DataTable QueryVirtualSoccer(LeagueGroup leagueGroup, SportType sportType, string spName)
        {
            var dicLeagueGroup = GetLeagueGroupParam();
            var leagueGroupParam = dicLeagueGroup[sportType][leagueGroup];

            var param = new Dictionary<string, object>
            {
                { "FromDate", dateService.FromDate },
                { "ToDate", dateService.ToDate},
                { "LeagueGroup", leagueGroupParam },
                { "SportType",  (int)sportType}
            };

            var db = new ObjectDb(spName);
            db.AssignParamValues(param);
            var dataSet = db.ExecuteDataSet();
            var datatable = new DataTable();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                datatable = dataSet.Tables[0];
            }

            return datatable;
        }

        private static Dictionary<SportType, Dictionary<LeagueGroup, string>> GetLeagueGroupParam()
        {
            return new Dictionary<SportType, Dictionary<LeagueGroup, string>>
            {
                {
                    SportType.Soccer, new Dictionary<LeagueGroup, string>
                    {
                        { LeagueGroup.Esoccer, "41" },
                        { LeagueGroup.VL, "42,74,108" },
                    }
                },
                {
                    SportType.Basketball, new Dictionary<LeagueGroup, string>
                    {
                        { LeagueGroup.Esoccer, "41" },
                        { LeagueGroup.VL, "42" },
                    }
                }
            };
        }
    }
}