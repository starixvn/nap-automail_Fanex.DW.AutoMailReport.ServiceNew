using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BettypeWith3rdProducts.Business.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface DataSplitterService
    {
        Dictionary<string, List<DataTable>> Execute(SourceMapping sourceMapping);
    }

    public class DataSplitterServiceImp : DataSplitterService
    {
        private readonly RawDataService dataService;
        private readonly int numberOfColumnsOnEachSection = 70;

        public DataSplitterServiceImp(RawDataService dataService)
        {
            this.dataService = dataService;
        }

        public Dictionary<string, List<DataTable>> Execute(SourceMapping sourceMapping)
        {
            var result = new Dictionary<string, List<DataTable>>();
            var dataSource = dataService.Query(sourceMapping);
            var index = 0;
            foreach (var product in dataSource)
            {
                if (index < 2)
                {
                    result.Add(product.Key, new List<DataTable> { product.Value });
                }
                else
                {
                    var sportSections = SplitSportSections(product.Value);
                    result.Add(product.Key, sportSections);
                }

                index++;
            }

            return result;
        }

        private List<DataTable> SplitSportSections(DataTable value)
        {
            var numberOfSections = GetNumberOfSections(value);
            var beginColumnIndex = 0;
            var endColumnIndex = 0;
            var lastRemovedIndex = 0;
            var sportSections = new List<DataTable>();

            for (var indexOfSection = 0; indexOfSection < numberOfSections; indexOfSection++)
            {
                if (indexOfSection == 0)
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
                lastRemovedIndex = CreateSportSection(value, beginColumnIndex, endColumnIndex, sportSections);
            }

            return sportSections;
        }

        private int CreateSportSection(DataTable value, int beginColumnIndex, int endColumnIndex, List<DataTable> sportSections)
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

            var sportSection = value.AsDataView().ToTable(false, columnNames.ToArray());
            sportSections.Add(sportSection);
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