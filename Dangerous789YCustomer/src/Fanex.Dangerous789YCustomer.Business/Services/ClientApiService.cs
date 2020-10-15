using Fanex.Dangerous789YCustomer.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fanex.Dangerous789YCustomer.Business.Services
{
    public interface ClientApiService
    {
        List<EvidenceModel> GetEvidence(string action);
    }

    public class ClientApiServiceImp : ClientApiService
    {
        public List<EvidenceModel> GetEvidence(string action)
        {
            var custEvidences = new List<EvidenceModel>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(AppConfigService.Config.FPSApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseResult = client.GetAsync(action).Result;
                if (responseResult.IsSuccessStatusCode)
                {
                    custEvidences = responseResult.Content.ReadAsAsync<List<EvidenceModel>>().Result;
                }
            }

            return custEvidences;
        }
    }
}