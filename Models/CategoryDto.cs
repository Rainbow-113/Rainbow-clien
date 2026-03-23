using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace Rainbow.Models
{
    public class CategoryDto
    {
        [ValidateNever]
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
