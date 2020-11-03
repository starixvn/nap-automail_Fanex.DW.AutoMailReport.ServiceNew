using Fanex.BigCreditIncrement789Y.Business.Criterias;
using Fanex.BigCreditIncrement789Y.Business.Models;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.BigCreditIncrement789Y.Business.Services
{
    public interface DataService
    {
        List<PunterModel> QueryBigCreditIncrement();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;
        private readonly DateTimeServiceProvider dateService;

        public DataServiceImp(IDynamicRepository repository, DateTimeServiceProvider dateService)
        {
            this.repository = repository;
            this.dateService = dateService;
        }

        public List<PunterModel> QueryBigCreditIncrement()
        {
            var criteria = new Criteria
            {
                FromDate = dateService.DateTime.FromDate,
                ToDate = dateService.DateTime.ToDate,
                IsTest = AppConfigService.Config.IsTest
            };

            return repository.Fetch<PunterModel>(criteria).ToList();
        }
    }
}