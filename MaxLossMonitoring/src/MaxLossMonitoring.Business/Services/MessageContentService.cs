using System;
using System.Collections.Generic;
using System.Linq;
using MaxLossMonitoring.Business.Models;

namespace MaxLossMonitoring.Business.Services
{
    public interface MessageContentService
    {
        Dictionary<int, List<string>> GenerateMessageContent(MonitorList dataSourceOnEachGroup);
    }

    public class MessageContentServiceImp : MessageContentService
    {
        private readonly ApplicationConfigurationService appConfig;

        public MessageContentServiceImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        public Dictionary<int, List<string>> GenerateMessageContent(MonitorList dataSourceOnEachGroup)
        {
            var result = new Dictionary<int, List<string>>();
            var messageSize = appConfig.Configurations.MessageSize;
            var pageSize = dataSourceOnEachGroup.Rows.Count / messageSize;
            pageSize += (dataSourceOnEachGroup.Rows.Count % messageSize) > 0 ? 1 : 0;
            var rowIndex = 1;
            for (var pageIndex = 0; pageIndex < pageSize; pageIndex++)
            {
                var rowsOfPage = dataSourceOnEachGroup.Rows.Skip(pageIndex * messageSize).Take(messageSize).ToList();
                List<string> contentOnEachPage = new List<string>();
                rowsOfPage.ForEach(row =>
                {
                    contentOnEachPage.Add($"{rowIndex}.{row.RoleNameForLetstalk}:{row.Username}[{row.Currency}]{Environment.NewLine}     {Truncate(row.TotalBalance, 0):#,###}/{Truncate(row.MaxLoss, 0):#,###} {Truncate(row.PercentageLoss, 0):#,###}%");
                    rowIndex++;
                });

                result.Add(pageIndex + 1, contentOnEachPage);
            }

            return result;
        }

        public static decimal Truncate(decimal value, int precision)
        {
            decimal step = Convert.ToDecimal(Math.Pow(10, precision));
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }
}