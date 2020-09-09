using Fanex.Dangerous789YCustomer.Business.Models;
using System.Collections.Generic;
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

        private List<PunterModel> GetRootMembers(List<PunterModel> data)
        {
            var rowPerBatch = RowPerBatch;
            var numberOfBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            for (var batchIndex = 0; batchIndex < numberOfBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var custIds = rowsOfBatch.Select(s => s.CustId).ToList();
                var strCustIds = string.Join(",", custIds);
                var action = string.Format(AppConfigService.Config.Action, strCustIds, "0");
                var result = api.GetEvidence(action);
            }

            return new List<PunterModel>();
        }

        private List<EvidenceModel> GetLastEvidence(List<EvidenceModel> data)
        {
        }

        public List<PunterModel> MarkEvidenceCustomer(List<PunterModel> data)
        {
            var rs1 = GetRootMembers(data);
            return new List<PunterModel>();
        }
    }
}