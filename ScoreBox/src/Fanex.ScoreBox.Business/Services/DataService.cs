using Fanex.Data.Repository;
using Fanex.ScoreBox.Business.Criterias;
using Fanex.ScoreBox.Business.Models;
using Fanex.Service.Core;
using System;
using System.Collections.Generic;

namespace Fanex.ScoreBox.Business.Services
{
    public interface DataService
    {
        IEnumerable<ScoreBoxModel> Execute();

        bool IsDataAvailable();

        LastDateModel QueryLastDate();
    }

    public class DataServiceImp : DataService
    {
        private readonly DateProvider dateProvider;
        private readonly IDynamicRepository dynamicRepository;

        public DataServiceImp(DateProvider dateProvider, IDynamicRepository dynamicRepository)
        {
            this.dateProvider = dateProvider;
            this.dynamicRepository = dynamicRepository;
        }

        public IEnumerable<ScoreBoxModel> Execute()
        {
            var date = dateProvider.Date(AppConfigService.AppConfig.IsDaily);
            var criteria = new ScoreBoxCriteria();
            criteria.FromDate = date.FromDate;
            criteria.ToDate = date.ToDate;

            return dynamicRepository.Fetch<ScoreBoxModel>(criteria);
        }

        public bool IsDataAvailable()
        {
            var lastdate = QueryLastDate();
            var result = false;
            if (AppConfigService.AppConfig.IsManual || !AppConfigService.AppConfig.IsCheckEnoughData)
            {
                return true;
            }

            result = lastdate.LastDate >= Convert.ToDateTime(SystemTime.Now.AddDays(-1).ToShortDateString());

            return result;
        }

        public LastDateModel QueryLastDate()
        {
            return dynamicRepository.Get<LastDateModel>(new GetDWStatusByCubeCriteria());
        }
    }
}