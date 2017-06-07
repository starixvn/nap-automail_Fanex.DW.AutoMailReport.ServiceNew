using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface SportSplitterService
    {
        Dictionary<string, List<DataTable>> Execute(SourceMapping sourceMapping);
    }

    public class SportSplitterServiceImp : SportSplitterService
    {
        private readonly RepositoryService repositoryService;
        private readonly int numberOfColumnsOnEachSection = 70;
        private readonly int firstIndexSportSection;

        public SportSplitterServiceImp(RepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
            firstIndexSportSection = 0;
        }

        public Dictionary<string, List<DataTable>> Execute(SourceMapping sourceMapping)
        {
            var result = new Dictionary<string, List<DataTable>>();
            SplitGeneralData(sourceMapping, result);
            SplitSportData(sourceMapping, result);

            return result;
        }

        private void SplitGeneralData(SourceMapping sourceMapping, Dictionary<string, List<DataTable>> result)
        {
            var generalData = repositoryService.QueryGeneralData(sourceMapping);
            foreach (var sport in generalData)
            {
                result.Add(sport.Key, new List<DataTable> { sport.Value });
            }
        }

        private void SplitSportData(SourceMapping sourceMapping, Dictionary<string, List<DataTable>> result)
        {
            var detailData = repositoryService.QueryDetailData(sourceMapping);
            foreach (var sport in detailData)
            {
                var sportSections = SplitSportIntoManySections(sport.Value);
                result.Add(sport.Key, sportSections);
            }
        }

        private List<DataTable> SplitSportIntoManySections(DataTable value)
        {
            var numberOfSections = GetNumberOfSections(value);
            var beginColumnIndex = 0;
            var endColumnIndex = 0;
            var lastRemovedIndex = 0;
            var sections = new List<DataTable>();

            for (var indexOfSportSection = 0; indexOfSportSection < numberOfSections; indexOfSportSection++)
            {
                if (indexOfSportSection == firstIndexSportSection)
                {
                    beginColumnIndex = 1;
                    endColumnIndex = numberOfColumnsOnEachSection + 1;
                }
                else
                {
                    beginColumnIndex = endColumnIndex - lastRemovedIndex;
                    endColumnIndex = beginColumnIndex + numberOfColumnsOnEachSection;
                }

                endColumnIndex = endColumnIndex > value.Columns.Count ? value.Columns.Count : endColumnIndex;
                lastRemovedIndex = AddSportIntoSection(value, beginColumnIndex, endColumnIndex, sections);
            }

            return sections;
        }

        private int AddSportIntoSection(DataTable value, int beginColumnIndex, int endColumnIndex, List<DataTable> sections)
        {
            var columnNames = new List<string>();
            var lastRemovedIndex = 0;
            columnNames.Add(value.Columns[0].ColumnName);
            for (int columnIndex = beginColumnIndex; columnIndex < endColumnIndex; columnIndex++)
            {
                var columnName = value.Columns[columnIndex].ColumnName;
                var currentBettype = columnName.Split(new[] { "@#@" }, StringSplitOptions.RemoveEmptyEntries)[0];
                columnNames.Add(columnName);
                if (columnIndex == endColumnIndex - 1)
                {
                    lastRemovedIndex = RemoveRedundantBettypes(columnNames, currentBettype);
                }
            }

            var section = value.AsDataView().ToTable(false, columnNames.ToArray());
            sections.Add(section);

            return lastRemovedIndex;
        }

        private int RemoveRedundantBettypes(List<string> columnNames, string currentBettype)
        {
            var lastRemovedIndex = 0;
            var numberOfBettypeOnEachGroup = GetNumberOfBettypeOnEachGroup(columnNames);
            var countBettypeGroup = CountBettypeGroup(columnNames, currentBettype);
            var isEnoughBettypeGroup = countBettypeGroup == numberOfBettypeOnEachGroup;
            if (!isEnoughBettypeGroup)
            {
                columnNames.RemoveRange(columnNames.Count - countBettypeGroup, countBettypeGroup);
                lastRemovedIndex = countBettypeGroup;
            }

            return lastRemovedIndex;
        }

        private static int CountBettypeGroup(List<string> columnNames, string currentBettype)
        {
            return columnNames
                .Where(s => s.Contains("@#@"))
                .Count(s => s.Split(new[] { "@#@" }, StringSplitOptions.RemoveEmptyEntries)[0] == currentBettype);
        }

        private int GetNumberOfBettypeOnEachGroup(List<string> columnNames)
        {
            var count = columnNames.Count(s => s.Contains("@#@"));
            if (count == 0)
            {
                return count;
            }

            return columnNames
                .Where(s => s.Contains("@#@"))
                .Select(s => s.Split(new[] { "@#@" }, StringSplitOptions.RemoveEmptyEntries)[0])
                .GroupBy(group => group, (key, value) => new { count = value.Count() })
                .Distinct()
                .Max(bettype => bettype.count);
        }

        private int GetNumberOfSections(DataTable value)
        {
            var columnCount = value.Columns.Count;
            var numberOfSections = columnCount / numberOfColumnsOnEachSection;
            if (columnCount % numberOfColumnsOnEachSection > 0)
            {
                numberOfSections++;
            }

            return numberOfSections;
        }
    }
}