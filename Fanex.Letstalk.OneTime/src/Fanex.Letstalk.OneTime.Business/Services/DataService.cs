using Fanex.Data.Repository;
using Fanex.Letstalk.OneTime.Business.Criterias;
using Fanex.Letstalk.OneTime.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Letstalk.OneTime.Business.Services
{
    public interface DataService
    {
        List<MailModel> QueryMail(DateTime rptDate, JobStatus jobStatus);
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository repository;

        public DataServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public List<MailModel> QueryMail(DateTime rptDate, JobStatus jobStatus)
        {
            var criteria = new CheckMailStatusCriteria
            {
                FromDate = rptDate,
                ToDate = rptDate,
                Status = (int)jobStatus
            };
            return repository.Fetch<MailModel>(criteria).ToList();
        }
    }
}