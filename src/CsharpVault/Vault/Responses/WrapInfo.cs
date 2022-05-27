using Newtonsoft.Json;

namespace NexusNetworkCloud.CsharpVault.Vault.Responses
{
    public class WrapInfo
    {
        [JsonProperty("token")]
        public string? Token { get; set; }

        [JsonProperty("ttl")]
        public int TTL { get; set; }

        [JsonProperty("creation_time")]
        public DateTime CreationTime { get; set; }

        [JsonProperty("creation_path")]
        public string? CreationPath { get; set; }
    }
}
