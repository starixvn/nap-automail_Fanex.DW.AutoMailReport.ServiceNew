using System;
using System.Collections.Generic;
using System.Linq;
using MaxLossMonitoring.Business.Models;

namespace MaxLossMonitoring.Business.Services
{
    public interface DataService
    {
        List<Level> QueryLevel();

        List<MonitorList> QueryMonitorListForLetstalk();

        Tuple<List<MonitorList>, List<MonitorListRaw>> QueryMonitorListForEmail();
    }

    public class DataServiceImp : DataService
    {
        private readonly MaxLossRepositoryService repository;

        public DataServiceImp(MaxLossRepositoryService repository)
        {
            this.repository = repository;
        }

        public List<Level> QueryLevel()
        {
            var result = repository.QueryLevel();

            Level higherLevel = null;
            result.ForEach(level =>
            {
                level.TextForLetstalk = $"{level.Value}";
                level.TextForEmail = higherLevel == null
                ? $"{level.Value}% & Above"
                : $"{level.Value}% - {higherLevel.Value - 1}%";

                higherLevel = level;
            });

            return result;
        }

        public List<MonitorList> QueryMonitorListForLetstalk()
        {
            var rawData = repository.QueryMonitorListForLetstalk();
            var result = QueryMonitorList(rawData);

            return result;
        }

        private List<MonitorList> QueryMonitorList(List<MonitorListRaw> rawData)
        {
            var result = new List<MonitorList>();
            rawData.ForEach(row =>
            {
                row.RoleNameForEmail = GetRoleNameForEmail(row.RoleId, row.GroupHeadlineId);
                row.RoleNameForLetstalk = GetRoleNameForLetstalk(row.RoleId, row.GroupHeadlineId);
            });

            var levels = QueryLevel();
            List<int> levelIds = rawData.Select(s => s.LevelId).Distinct().ToList();
            levelIds.ForEach(levelId =>
            {
                var dataByLevelId = rawData.Where(s => s.LevelId == levelId).ToList();
                var level = levels.FirstOrDefault(s => s.LevelId == levelId) ?? new Level();
                result.Add(new MonitorList
                {
                    Rows = dataByLevelId,
                    LevelId = level.LevelId,
                    LevelNameForEmail = level.TextForEmail,
                    LevelNameForLetstalk = level.TextForLetstalk
                });
            });

            return result;
        }

        private static string GetRoleNameForEmail(int roleId, int groupHeadlineId)
        {
            var level = string.Empty;
            var levels = new Dictionary<int, string>
            {
                { 1, "Member" },
                { 2, "Agent" },
                { 3, "Master" },
                { 4, "Super" }
            };

            level = levels.ContainsKey(roleId) ? levels[roleId] : string.Empty;
            level = groupHeadlineId != 0 ? "Headline" : level;

            return level;
        }

        private static string GetRoleNameForLetstalk(int roleId, int groupHeadlineId)
        {
            var level = string.Empty;
            var levels = new Dictionary<int, string>
            {
                { 1, "M" },
                { 2, "A" },
                { 3, "M" },
                { 4, "S" }
            };

            level = levels.ContainsKey(roleId) ? levels[roleId] : string.Empty;
            level = groupHeadlineId != 0 ? "H" : level;

            return level;
        }

        public Tuple<List<MonitorList>, List<MonitorListRaw>> QueryMonitorListForEmail()
        {
            var tuple = repository.QueryMonitorListForEmail();
            var main = QueryMonitorList(tuple.Item1.ToList());
            var detail = tuple.Item2.ToList();
            detail.ForEach(row =>
            {
                row.RoleNameForEmail = GetRoleNameForEmail(row.RoleId, 0);
            });

            var result = new Tuple<List<MonitorList>, List<MonitorListRaw>>(main, detail);

            return result;
        }
    }
}