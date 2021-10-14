using Newtonsoft.Json;

namespace AzureFunctions.Models
{
    public class Calculation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
    }
}