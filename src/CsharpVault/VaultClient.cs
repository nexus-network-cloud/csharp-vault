using Microsoft.Extensions.Logging;
using NexusNetworkCloud.CsharpVault.Authentication;
using NexusNetworkCloud.CsharpVault.Secrets.KV;
using NexusNetworkCloud.CsharpVault.Vault;

namespace NexusNetworkCloud.CsharpVault
{
    public interface IVaultClient
    {
    }

    public class VaultClient : IVaultClient, IDisposable
    {
        private readonly ILogger? _logger;
        
        private readonly IVaultConnectionHandler _connectionHandler;
        
        public IVaultCredentialHandler Authentication { get; private set; }

        // Authentication
        public IUserpass Userpass { get; private set; }

        // Backend

        // Secrets
        public IKV2 KV2 { get; private set; }

        public VaultClient(VaultConnectionConfiguration connectionConfig, ILogger? logger = null)
        {
            _logger = logger;

            Authentication = new VaultCredentialHandler(connectionConfig, logger);

            _connectionHandler = new VaultConnectionHandler(connectionConfig, Authentication, logger);

            // Authentication
            Userpass = new Userpass(_connectionHandler);
            
            // Backend

            // Secrets
            KV2 = new KV2(_connectionHandler);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            _connectionHandler.Dispose();
        }
    }
}