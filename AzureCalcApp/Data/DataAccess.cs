using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AzureCalcApp.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly HttpClient _httpClient = new();

        private readonly SecretClientOptions _options = new()
        {
            Retry =
            {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };
        public async Task<string> Calculate(string numbers)
        {
            if (string.IsNullOrWhiteSpace(numbers))
                return null;
            string code = string.Empty;
            var json = JsonSerializer.Serialize(numbers);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            if (numbers.Contains('+')) {
                code = await GetSecret("AddNumbers");
                var addResult = await _httpClient.PostAsync($"https://bjorkdev-calculator.azurewebsites.net/api/AddNumbers?code={code}", data);
                
                if(addResult.IsSuccessStatusCode)
                    return await addResult.Content.ReadAsStringAsync();
                return null;
            }
            code = await GetSecret("SubNumbers");
            var subResult = await _httpClient.PostAsync($"https://bjorkdev-calculator.azurewebsites.net/api/SubNumbers?code={code}", data);
            
            if(subResult.IsSuccessStatusCode)
                return await subResult.Content.ReadAsStringAsync();
            return null;
        }

        public async Task<Calculation[]> GetNumbers()
        {
            string code = await GetSecret("GetCalculations");
            var response = await _httpClient.GetAsync($"https://bjorkdev-calculator.azurewebsites.net/api/GetCalculations?code={code}");

            if(!response.IsSuccessStatusCode)
                return null;

            var queue = await response.Content.ReadFromJsonAsync<Calculation[]>();
            return queue;
        }

        private async Task<string> GetSecret(string secretName)
        {
            var azure = new DefaultAzureCredentialOptions { ExcludeVisualStudioCredential = true };
            var client = new SecretClient(new Uri("https://bjorkdev.vault.azure.net/"), new DefaultAzureCredential(azure), _options);
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}
