using Fanex.Dangerous789YCustomer.Business.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface EvidenceAccountService
    {
        List<PunterModel> MarkEvidenceCustomer(List<PunterModel> data);
    }

    public class EvidenceAccountServiceImp : EvidenceAccountService
    {
        private readonly ClientApiService api;
        private const int RowPerBatch = 100;

        public EvidenceAccountServiceImp(ClientApiService api)
        {
            this.api = api;
        }

        private List<PunterModel> GetRootCustomer(List<PunterModel> data)
        {
            var punters = new List<PunterModel>();
            var evidenceCustomerFromApi = new List<EvidenceModel>();
            var rowPerBatch = RowPerBatch;
            var numberOfBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var associatedLevel = "0";
            for (var batchIndex = 0; batchIndex < numberOfBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var custIds = rowsOfBatch.Select(s => s.CustId).ToList();
                var strCustIds = string.Join(",", custIds);
                var action = string.Format(AppConfigService.Config.Action, strCustIds, associatedLevel);
                var evidenceCustomer = api.GetEvidence(action);
                evidenceCustomerFromApi.AddRange(evidenceCustomer);
            }

            var evidenceCustomers = data.Where(s => evidenceCustomerFromApi.Select(e => e.CustId).Contains(s.CustId)).ToList();
            evidenceCustomers.ForEach(row =>
            {
                var evidence = evidenceCustomerFromApi.FirstOrDefault(s => s.CustId == row.CustId);
                var punter = Transform(true, row, evidence);
                punters.Add(punter);
            });

            return punters.OrderByDescending(s => s.Winloss1).ToList();
        }

        private List<PunterModel> GetAssociatedMembersCustomer(int custId, List<PunterModel> data)
        {
            var punters = new List<PunterModel>();
            var associatedLevel = "1";
            var action = string.Format(AppConfigService.Config.Action, custId, associatedLevel);
            var evidenceCustomer = api.GetEvidence(action);
            var evidenceCustomers = data.Where(s => evidenceCustomer.Select(e => e.CustId).Contains(s.CustId)).ToList();
            evidenceCustomers.ForEach(row =>
            {
                var evidence = evidenceCustomer.FirstOrDefault(s => s.CustId == row.CustId);
                var punter = Transform(false, row, evidence);
                punters.Add(punter);
            });

            return punters.OrderByDescending(s => s.Winloss1).ToList();
        }

        private static PunterModel Transform(bool isRootMember, PunterModel row, EvidenceModel evidence)
        {
            return new PunterModel
            {
                UserName = row.UserName,
                CurrencyName = row.CurrencyName,
                Turnover1 = row.Turnover1,
                Winloss1 = row.Winloss1,
                Turnover2 = row.Turnover2,
                Winloss2 = row.Winloss2,
                Turnover3 = row.Turnover3,
                Winloss3 = row.Winloss3,
                Evidence = $"{evidence.EvidenceCode} {evidence.EvidenceName}",
                CustId = row.CustId,
                IsRootMember = isRootMember,
                Level0CustId = evidence.Level0CustId
            };
        }

        public List<PunterModel> MarkEvidenceCustomer(List<PunterModel> data)
        {
            var punters = new List<PunterModel>();
            var rootMembers = GetRootCustomer(data);

            rootMembers.ForEach(row =>
            {
                var subAssociatedAccount = GetAssociatedMembersCustomer(row.CustId, data);
                subAssociatedAccount.ForEach(sub =>
                {
                    sub.AssociatedAccount = row.UserName;
                });

                punters.Add(row);
                punters.AddRange(subAssociatedAccount);
            });
            return punters;
        }
    }
}