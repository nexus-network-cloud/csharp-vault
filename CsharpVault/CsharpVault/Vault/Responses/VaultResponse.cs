using Newtonsoft.Json;

namespace NexusNetworkCloud.CsharpVault.Vault.Responses
{
    public class NoContentVaultResponse
    {
        public bool RequestSuccessful { get; set; }
    }

    public class VaultResponse
    {
        [JsonProperty("request_id")]
        public string? RequestId { get; set; }

        [JsonProperty("lease_id")]
        public string? LeaseId { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }

        [JsonProperty("lease_duration")]
        public long LeaseDuration { get; set; }

        [JsonProperty("wrap_info")]
        public WrapInfo? WrapInfo { get; set; }

        [JsonProperty("warnings")]
        public List<string>? Warnings { get; set; }

        [JsonProperty("errors")]
        public List<string>? Errors { get; set; }
    }

    public class VaultResponse<T> : VaultResponse
    {
        [JsonProperty("data")]
        public T? Data { get; set; }
    }
}
