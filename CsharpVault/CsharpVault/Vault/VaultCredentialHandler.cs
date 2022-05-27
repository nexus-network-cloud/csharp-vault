using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.CsharpVault.Vault
{
    public interface IVaultCredentialHandler
    {
        public string? Token { get; }

        /// <summary>
        /// Logs a user into Vault using the userpass authentication method. Token information will automatically be stored and used for future calls.
        /// </summary>
        /// <param name="username">The username of the configured user within the userpass mount.</param>
        /// <param name="password">The password of the confgiured user within the userpass mount.</param>
        /// <param name="mount">The userpass mount where the user is configured. Defaults to userpass.</param>
        /// <returns>Whether or not the login was successful.</returns>
        public Task<bool> LoginUserpassAsync(string username, string password, string mount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="secretId"></param>
        /// <param name="mount"></param>
        /// <returns></returns>
        Task<bool> LoginApproleAsync(string roleId, string? secretId = null, string? mount = null);
    }

    internal class VaultCredentialHandler : IVaultCredentialHandler, IDisposable
    {
        private readonly VaultConnectionConfiguration _connectionConfiguration;
        private readonly HttpClient _client;
        private readonly ILogger? _logger;

        private VaultAuthenticationInfo _authenticationInfo;
        private string baseApiPath { get { return $"{_connectionConfiguration.VaultAddress}:{_connectionConfiguration.Port}"; } }

        public string? Token { get { return _authenticationInfo.Token; } }

        public VaultCredentialHandler(VaultConnectionConfiguration connectionConfig, ILogger? logger)
        {
            _logger = logger;
            _authenticationInfo = new();
            _connectionConfiguration = connectionConfig;

            _client = _connectionConfiguration.CustomHttpClientHandler is null ? new HttpClient() : new HttpClient(_connectionConfiguration.CustomHttpClientHandler);
            
        }

        private async Task<HttpResponseMessage> SendVaultLoginRequestAsync(string apiEndpoint, JObject? payload = null)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseApiPath}/v1/{apiEndpoint}"))
            {
                if (payload is not null)
                    request.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

                return await _client.SendAsync(request);
            }
        }

        public async Task<bool> LoginUserpassAsync(string username, string password, string mount = "userpass")
        {
            JObject payload = new JObject { { "password", password } };

            HttpResponseMessage vaultResponse = await SendVaultLoginRequestAsync($"auth/{mount}/login/{username}", payload);

            if (vaultResponse.IsSuccessStatusCode)
            {
                try
                {
                    _authenticationInfo = JObject.Parse(await vaultResponse.Content.ReadAsStringAsync()).Value<JObject>("auth")?.ToObject<VaultAuthenticationInfo>() ?? new();
                }
                catch (JsonException jsonException)
                {
                    _logger?.LogError(jsonException, "Failed to parse authentication data into known format. Login succeeded");
                    return false;
                }

                return true;
            }
            else
            {
                string error = await vaultResponse.Content.ReadAsStringAsync();
                _logger?.LogError($"Failed to login as user {username} to the userpass authentication method located at mount: {mount}");
                return false;
            }
        }

        public async Task<bool> LoginApproleAsync(string roleId, string? secretId = null, string? mount = "approle")
        {
            JObject payload = new JObject { { "role_id", roleId }, { "secret_id", secretId } };
            HttpResponseMessage vaultResponse = await SendVaultLoginRequestAsync($"auth/{mount}/login", payload);

            if (vaultResponse.IsSuccessStatusCode)
            {
                try
                {
                    _authenticationInfo = JObject.Parse(await vaultResponse.Content.ReadAsStringAsync()).Value<JObject>("auth")?.ToObject<VaultAuthenticationInfo>() ?? new();
                }
                catch (JsonException jsonException)
                {
                    _logger?.LogError(jsonException, "Failed to parse authentication data into known format. Login succeeded");
                    return false;
                }

                return true;
            }
            else
            {
                string error = await vaultResponse.Content.ReadAsStringAsync();
                _logger?.LogError($"Failed to login as role-id {roleId} to the approle authentication method located at mount: {mount}");
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_connectionConfiguration.AutoTokenRevocationOnShutdown)
                _logger?.LogDebug("Attempting To Revoke Token Here As Token Will No Longer Be Managed By Servcie...");

            _client.Dispose();
        }
    }
}
