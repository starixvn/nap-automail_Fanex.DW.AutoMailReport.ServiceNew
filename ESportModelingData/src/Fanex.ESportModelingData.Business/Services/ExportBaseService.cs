using Fanex.ESportModelingData.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ExportBaseService
    {
        void ExporDimension(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups);

        string Replace(string name);
    }

    public class ExportBaseServiceImp : ExportBaseService
    {
        private readonly List<string> specialCharacters;
        private readonly CsvService csvService;

        public ExportBaseServiceImp(CsvService csvService)
        {
            this.csvService = csvService;
            specialCharacters = new List<string>
            {
                @"\", "/", "*", "?", "<", ">", "|", ":"
            };
        }

        public void ExporDimension(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        {
            name = Replace(name);

            ExportBetCount(path, name, status, statusGroups);
            //ExportRealIncome(path, name, status, statusGroups);
            ExportRealTurnover(path, name, status, statusGroups);
            ExportVirtualWinLoss(path, name, status, statusGroups);
            ExportVirtualrateSetting(path, name, status, statusGroups);
        }

        private void ExportBetCount(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        {
            var fileName = $@"{path}\{name} - Bet Count - {status}.csv";
            var data = new List<BetCountModel>();
            statusGroups.ToList()
            .ForEach(m =>
            {
                data.Add(new BetCountModel
                {
                    Customer = m.Customer,
                    BetCount = m.BetCount
                });
            });

            csvService.Execute(data, fileName);
        }

        //private void ExportRealIncome(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        //{
        //    var fileName = $@"{path}\{name} - Real Win Loss - {status}.csv";
        //    var data = new List<RealIncomeModel_>();
        //    statusGroups.ToList()
        //    .ForEach(m =>
        //    {
        //        data.Add(new RealIncomeModel_
        //        {
        //            Customer = m.Customer,
        //            //RealIncome = m.RealIncome
        //        });
        //    });

        //    csvService.Execute(data, fileName);
        //}

        private void ExportRealTurnover(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        {
            var fileName = $@"{path}\{name} - Real Turnover - {status}.csv";
            var data = new List<MemberTurnoverModel>();
            statusGroups.ToList()
            .ForEach(m =>
            {
                data.Add(new MemberTurnoverModel
                {
                    Customer = m.Customer,
                    MemberTurnover = m.MemberTurnover
                });
            });

            csvService.Execute(data, fileName);
        }

        private void ExportVirtualWinLoss(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        {
            var fileName = $@"{path}\{name} - Virtual Win Loss - {status}.csv";
            var data = new List<MemberWinLossModel>();
            statusGroups.ToList()
            .ForEach(m =>
            {
                data.Add(new MemberWinLossModel
                {
                    Customer = m.Customer,
                    MemberWinLoss = m.MemberWinLoss
                });
            });

            csvService.Execute(data, fileName);
        }

        private void ExportVirtualrateSetting(string path, string name, string status, IGrouping<dynamic, RawModelingDataModel> statusGroups)
        {
            var fileName = $@"{path}\{name} - Virtualrate Setting - {status}.csv";
            var data = new List<VirtualrateSettingModel>();
            statusGroups.ToList()
            .ForEach(m =>
            {
                data.Add(new VirtualrateSettingModel
                {
                    Customer = m.Customer,
                    VirtualrateSetting = m.VirtualrateSetting
                });
            });

            csvService.Execute(data, fileName);
        }

        public string Replace(string name)
        {
            specialCharacters.ForEach(s =>
            {
                name = name.Replace(s, " ");
            });

            return name;
        }
    }
}