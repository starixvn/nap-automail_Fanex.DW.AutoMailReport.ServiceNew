using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Fanex.Service.Core;
using MaxLossMonitoring.Business.Models;
using MaxLossMonitoring.Business.Repositories;

namespace MaxLossMonitoring.Business.Services
{
    public interface MaxLossRepositoryService
    {
        List<Level> QueryLevel();

        List<MonitorListRaw> QueryMonitorListForLetstalk();

        Tuple<IEnumerable<MonitorListRaw>, IEnumerable<MonitorListRaw>> QueryMonitorListForEmail();
    }

    public class MaxLossRepositoryServiceImp : MaxLossRepositoryService, RepositoryService
    {
        private readonly IDynamicRepository repository;

        public MaxLossRepositoryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public bool IsDataAvailable()
        {
            return true;
        }

        public DateTime QueryLastDate()
        {
            return SystemTime.Now;
        }

        public List<Level> QueryLevel()
        {
            return repository.Fetch<Level>(new ActionConfigSelCriteria())
                .OrderByDescending(s => s.LevelId)
                .ToList();
        }

        public List<MonitorListRaw> QueryMonitorListForLetstalk()
        {
            return repository.Fetch<MonitorListRaw>(new MonitorListCriteria())
                .OrderByDescending(s => s.LevelId)
                .ToList();
        }

        public Tuple<IEnumerable<MonitorListRaw>, IEnumerable<MonitorListRaw>> QueryMonitorListForEmail()
        {
            return repository.FetchMultiple<MonitorListRaw, MonitorListRaw>(new MonitorListDetailCrieria());                
        }
    }
}