using Microsoft.Extensions.Logging;
using NexusNetworkCloud.CsharpVault.Vault.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.CsharpVault.Vault
{
    public interface IVaultConnectionHandler
    {
        internal void Dispose();
        internal Task<HttpResponseMessage> SendVaultRequestAsync(VaultRequest request);
    }

    internal class VaultConnectionHandler : IVaultConnectionHandler, IDisposable
    {
        private readonly VaultConnectionConfiguration _configuration;
        private readonly ILogger? _logger;
        private readonly HttpClient _client;

        private string baseApiPath { get { return $"{_configuration.VaultAddress}:{_configuration.Port}"; } }
        private readonly IVaultCredentialHandler _credentialHandler;

        public VaultConnectionHandler(VaultConnectionConfiguration configuration, IVaultCredentialHandler credentialHandler, ILogger? logger)
        {
            _configuration = configuration;
            _credentialHandler = credentialHandler;
            _logger = logger;

            if (_configuration.CustomHttpClientHandler is not null)
                _client = new HttpClient(_configuration.CustomHttpClientHandler);
            else
                _client = new HttpClient();

            _client.Timeout = TimeSpan.FromMilliseconds(_configuration.HttpClientTimeout);
        }

        public async Task<HttpResponseMessage> SendVaultRequestAsync(VaultRequest request)
        {
            using (HttpRequestMessage vaultRequest = new HttpRequestMessage(request.HttpMethod, $"{baseApiPath}/v1/{request.ApiEndpoint}"))
            {
                if (request.RequestPayload is not null)
                    vaultRequest.Content = new StringContent(request.RequestPayload.ToString(), Encoding.UTF8, request.RequestPayloadDataType);

                vaultRequest.Headers.Add("X-Vault-Token", _credentialHandler.Token);

                return await _client.SendAsync(vaultRequest);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) 
        {
            _client.Dispose();
        }
    }
}
