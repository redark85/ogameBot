using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaBot.Domain.Dtos
{
    public class TokenResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;

        [JsonProperty("isPlatformLogin")]
        public bool IsPlatformLogin { get; set; }

        [JsonProperty("isGameAccountMigrated")]
        public bool IsGameAccountMigrated { get; set; }

        [JsonProperty("platformUserId")]
        public string PlatformUserId { get; set; } = string.Empty;

        [JsonProperty("isGameAccountCreated")]
        public bool IsGameAccountCreated { get; set; }

        [JsonProperty("hasUnmigratedGameAccounts")]
        public bool HasUnmigratedGameAccounts { get; set; }
    }
}
