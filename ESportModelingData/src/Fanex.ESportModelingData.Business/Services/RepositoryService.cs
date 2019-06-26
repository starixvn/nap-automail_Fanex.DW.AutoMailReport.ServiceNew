using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Fanex.ESportModelingData.Business.Criterias;
using Fanex.ESportModelingData.Business.Models;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface RepositoryService
    {
        List<RawModelingDataModel> Query(DateTime dateStart, DateTime dateEnd);
    }

    public class RepositoryServiceImp : RepositoryService
    {
        private readonly IDynamicRepository repository;

        public RepositoryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public List<RawModelingDataModel> Query(DateTime dateStart, DateTime dateEnd)
        {
            var criteria = new ESportModelingDataCriteria
            {
                DateStart = dateStart,
                DateEnd = dateEnd
            };

            return repository.Fetch<RawModelingDataModel>(criteria).ToList();
        }
    }
}