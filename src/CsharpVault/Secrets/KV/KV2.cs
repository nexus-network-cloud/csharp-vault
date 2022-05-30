using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NexusNetworkCloud.CsharpVault.Vault;
using NexusNetworkCloud.CsharpVault.Vault.Requests;
using NexusNetworkCloud.CsharpVault.Vault.Responses;

namespace NexusNetworkCloud.CsharpVault.Secrets.KV
{
    public interface IKV2
    {
        void SetMount(string mount);
        Task<NoContentVaultResponse> ConfigureAsync(KV2Configuration config);
        Task<VaultResponse<KV2Configuration>> ReadConfigurationAsync();
        Task<VaultResponse<KV2Secret>> ReadSecretAsync(string path, int version = 0);
        Task<VaultResponse<KV2SecretMetadata>> CreateSecretAsync(string path, JObject data, int version = 0);
    }

    public class KV2 : IKV2
    {
        private readonly IVaultConnectionHandler _connectionHandler;
        private string mount = "secret";

        internal KV2(IVaultConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public void SetMount(string mount)
        {
            this.mount = mount;
        }

        public async Task<NoContentVaultResponse> ConfigureAsync(KV2Configuration config)
        {
            var vaultResponse = await _connectionHandler.SendVaultRequestAsync(new VaultRequest { HttpMethod = HttpMethod.Post, ApiEndpoint = $"{mount}/config", RequestPayload = JObject.FromObject(config) });

            return new NoContentVaultResponse { RequestSuccessful = vaultResponse.IsSuccessStatusCode };
        }

        public async Task<VaultResponse<KV2Configuration>> ReadConfigurationAsync()
        {
            var vaultResponse = await _connectionHandler.SendVaultRequestAsync(new VaultRequest { HttpMethod = HttpMethod.Get, ApiEndpoint = $"{mount}/config" });

            return JObject.Parse(await vaultResponse.Content.ReadAsStringAsync()).ToObject<VaultResponse<KV2Configuration>>() ?? new VaultResponse<KV2Configuration>();
        }

        public async Task<VaultResponse<KV2Secret>> ReadSecretAsync(string path, int version = 0)
        {
            var vaultResponse = await _connectionHandler.SendVaultRequestAsync(new VaultRequest { HttpMethod = HttpMethod.Get, ApiEndpoint = $"{mount}/data/{path}?version={version}"});

            return JObject.Parse(await vaultResponse.Content.ReadAsStringAsync()).ToObject<VaultResponse<KV2Secret>>() ?? new VaultResponse<KV2Secret>();
        }

        public async Task<VaultResponse<KV2SecretMetadata>> CreateSecretAsync(string path, JObject data, int version = 0)
        {
            var vaultResponse = await _connectionHandler.SendVaultRequestAsync(new VaultRequest { HttpMethod = HttpMethod.Patch, ApiEndpoint = $"{mount}/data/{path}", RequestPayload = new JObject { { "data", data } } });

            return JObject.Parse(await vaultResponse.Content.ReadAsStringAsync()).ToObject<VaultResponse<KV2SecretMetadata>>() ?? new VaultResponse<KV2SecretMetadata>();
        }
    }

    public class KV2Configuration
    {
        [JsonProperty("cas_required")]
        public bool CasRequired { get; set; } = false;

        [JsonProperty("delete_versions_after")]
        public int DeleteVersionsAfter { get; set; } = 0;

        [JsonProperty("max_versions")]
        public int MaxVersions { get; set; } = 0;
    }

    public class KV2Secret
    {
        [JsonProperty("data")]
        public JObject? SecretData { get; set; }

        [JsonProperty("metadata")]
        KV2SecretMetadata Metadata { get; set; } = new();
    }

    public class KV2SecretMetadata
    {
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("custom_metadata")]
        public JObject? CustomMetadata { get; set; }

        [JsonProperty("deletion_time")]
        public DateTime? DeletionTime { get; set; }

        [JsonProperty("destroyed")]
        public bool Destroyed { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
