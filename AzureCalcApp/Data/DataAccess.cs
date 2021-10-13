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
        private readonly HttpClient _httpClient = new();
        public async Task<string> Calculate(string numbers)
        {
            var json = JsonSerializer.Serialize(numbers);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            if (numbers.Contains('+')) {
                var addResult =  await _httpClient.PostAsync("http://localhost:7071/api/AddNumbers", data);
                return await addResult.Content.ReadAsStringAsync();
            }

            var subResult = await _httpClient.PostAsync("http://localhost:7071/api/SubNumbers", data);
            return await subResult.Content.ReadAsStringAsync();
        }
    }
}
