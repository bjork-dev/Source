using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureCalcApp.Data
{
    public interface IDataAccess
    {
        Task<string> Calculate(string numbers);
        Task<Queue<string>> GetNumbers();
    }
}
