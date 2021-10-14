using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureCalcApp.Data;

namespace AzureCalcApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public string Result { get; set; }
        public Queue<Calculation> Results { get; set; } = new();
        private readonly IDataAccess _data;

        public IndexModel(ILogger<IndexModel> logger, IDataAccess data)
        {
            _logger = logger;
            _data = data;
        }

        public async Task OnGetAsync()
        {
            Results = await _data.GetNumbers();
            Results.ValidateQueue(); 
        }
        public async Task OnPostAsync(string result)
        {
            var response = await _data.Calculate(result);
            if (response is null)
            {
                return;
            }
            else
            {
                return;
                // Results.Enqueue(response);
                // Results.ValidateQueue(); // Simple extension method for not allowing more than 10 items in the queue.
            }

        }
    }
}
