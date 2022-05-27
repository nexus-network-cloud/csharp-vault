using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NexusNetworkCloud.CsharpVault
{
    public class VaultAuthenticationInfo
    {
        [JsonProperty("client_token")]
        public string? Token { get; set; }

        [JsonProperty("accessor")]
        public string? Accessor { get; set; }

        [JsonProperty("policies")]
        public List<string> Policies { get; set; } = new();

        [JsonProperty("token_policies")]
        public List<string> TokenPolicies { get; set; } = new();

        [JsonProperty("identity_policies")]
        public List<string> IdentityPolicies { get; set; } = new();

        [JsonProperty("metadata")]
        public JObject? Metadata { get; set; }

        [JsonProperty("lease_duration")]
        public long LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("entity_id")]
        public string? EntityId { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("orphan")]
        public bool Orphan { get; set; }

        //[JsonProperty("mfa_requirement")] Not covered at this time
        //public object MfaRequiredment { get; set; }

        [JsonProperty("num_uses")]
        public int NumUses { get; set; }
    }
}
