using Fanex.SuspiciousAccount.Business.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SuspiciousAccount.Business.Services
{
    public interface EvidenceAccountService
    {
        List<AccountModel> GetEvidenceAccount(List<AccountModel> accounts);
    }

    public class EvidenceAccountServiceImp : EvidenceAccountService
    {
        private readonly ClientApiService webApiService;

        public EvidenceAccountServiceImp(ClientApiService webApiService)
        {
            this.webApiService = webApiService;
        }

        public List<AccountModel> GetEvidenceAccount(List<AccountModel> accounts)
        {
            var custIds = accounts.Select(s => s.CustId).ToList();
            var action = "CustomersEvidence100/{0}";
            action = string.Format(action, string.Join(",", custIds));
            var result = webApiService.GetEvidence(action);
            return accounts.Where(s => result.Contains(s.CustId)).ToList();
        }
    }
}