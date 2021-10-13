using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureCalcApp.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly IHttpClientFactory _httpClient;

        public DataAccess(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsync(string numbers)
        {
            var client = _httpClient.CreateClient();
            var json = JsonSerializer.Serialize(numbers);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync("", data);
        }

        public async Task<string> GetAsync()
        {
            var client = _httpClient.CreateClient();
            var response = await client.GetAsync("");
            return await response.Content.ReadAsStringAsync();
        }
    }
}
