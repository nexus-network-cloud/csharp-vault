using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.CsharpVault.Vault
{
    public class VaultConnectionConfiguration
    {
        public string VaultAddress { get; set; } = "http://localhost";
        public int Port { get; set; } = 8200;
        public string? InitialToken { get; set; }
        public HttpClientHandler? CustomHttpClientHandler { get; set; }
        public bool AutoTokenRevocationOnShutdown { get; set; } = true;

        /// <summary>
        /// The timeout, in milliseconds, for the HttpClient that handles requests to Vault.
        /// </summary>
        public double HttpClientTimeout { get; set; } = 1000;
    }
}
