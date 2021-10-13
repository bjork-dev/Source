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

        private readonly IDataAccess _data;

        public IndexModel(ILogger<IndexModel> logger, IDataAccess data)
        {
            _logger = logger;
            _data = data;
        }

        public void OnGet()
        {

        }

        public async Task<RedirectToPageResult> OnPostAsync(string result)
        {
            var response = await _data.PostAsync(result);
            return RedirectToPage("Index");
        }

    }
}
