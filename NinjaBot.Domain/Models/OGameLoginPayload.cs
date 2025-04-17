using System.Text.Json.Serialization;

namespace NinjaBot.Domain.Models
{
    public class OGameLoginPayload
    {
        [JsonPropertyName("blackbox")]
        public string Blackbox { get; set; } = string.Empty;

        [JsonPropertyName("gameEnvironmentId")]
        public string GameEnvironmentId { get; set; } = string.Empty;
        
        [JsonPropertyName("gfLang")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("identity")]
        public string Identity { get; set; } = string.Empty;

        [JsonPropertyName("locale")]
        public string Locale { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("platformGameId")]
        public string PlatformGameId { get; set; } = string.Empty;
    }
}
