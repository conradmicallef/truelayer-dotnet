using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TrueLayer.Payments.Model;

namespace TrueLayer.AcceptanceTests


{
    public static class ConsumerApiClient
    {
        static public async Task<HttpResponseMessage> postCreatePaymentAPI(string baseUri,
            paymentRequestModel.paymentRequest payRequest)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(baseUri)})
            {

                string json = JsonConvert.SerializeObject(payRequest);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = await client.PostAsync($"/payments", httpContent);
                    return response;
                }
                catch (System.Exception ex)
                {
                    throw new Exception("There was a problem connecting to Provider API.", ex);
                }
            }
        }
    }
}

