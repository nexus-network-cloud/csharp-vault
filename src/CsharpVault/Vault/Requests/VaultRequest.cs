using Newtonsoft.Json.Linq;

namespace NexusNetworkCloud.CsharpVault.Vault.Requests
{
    internal class VaultRequest
    {
        internal HttpMethod HttpMethod { get; set; } = HttpMethod.Get;
        internal string ApiEndpoint { get; set; } = "/";
        internal JObject? RequestPayload { get; set; }
        internal string RequestPayloadDataType { get; set; } = "application/json";
    }
}
