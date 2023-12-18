using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExampleLongPollingWithTaskCompletionSource.Api.Models
{
    public class GetAllOrderRequest
    {
        [FromQuery(Name = "serial_number")]
        [JsonProperty("serial_number")]
        public string? SerialNumber { get; set; }

        [FromQuery(Name = "id")]
        [JsonProperty("id")]
        public string? Id { get; set; }
    }
}
