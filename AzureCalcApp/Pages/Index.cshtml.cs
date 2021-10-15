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
        public Calculation[] Results { get; set; } = {};
        private readonly IDataAccess _data;

        public IndexModel(ILogger<IndexModel> logger, IDataAccess data)
        {
            _logger = logger;
            _data = data;
        }

        public async Task OnGetAsync()
        {
            Results = await _data.GetNumbers();
            //Results.ValidateQueue();
        }
        public async Task OnPostAsync(string result)
        {
            try
            {
                var response = await _data.Calculate(result);
                if (response is null)
                {
                    await OnGetAsync();
                }
            }
            catch
            {
                await OnGetAsync();

            }
            await OnGetAsync();
        }
    }
}
