using System;
using Newtonsoft.Json;

namespace AzureCalcApp.Data
{
    public class Calculation //Model from database
    {
        public string Id { get; set; }
        public string Result { get; set; }
        public DateTime RunDate { get; set; }
    }
}
