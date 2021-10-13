using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureCalcApp.Data
{
    public interface IDataAccess
    {
        Task<HttpResponseMessage> PostAsync(string numbers);
        Task<string> GetAsync();
    }
}
