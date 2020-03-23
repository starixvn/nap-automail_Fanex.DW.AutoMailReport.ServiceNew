using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Fanex.SuspiciousAccount.Business.Services
{
    public interface ClientApiService
    {
        List<int> GetEvidence(string action);
    }

    public class ClientApiServiceImp : ClientApiService
    {
        public List<int> GetEvidence(string action)
        {
            var custEvidences = new List<int>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(AppConfigService.Config.FPSApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseResult = client.GetAsync(action).Result;
                if (responseResult.IsSuccessStatusCode)
                {
                    var data = responseResult.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                    if(string.IsNullOrEmpty(data))
                    {
                        return custEvidences;
                    }

                    data.Split(',').ToList().ForEach(custId =>
                    {
                        custEvidences.Add(Convert.ToInt32(custId));
                    });
                }
            }

            return custEvidences;
        }
    }
}