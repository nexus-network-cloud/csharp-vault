using NexusNetworkCloud.CsharpVault.Vault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusNetworkCloud.CsharpVault.Authentication
{
    public interface IUserpass
    {
    }

    public class Userpass : IUserpass
    {
        private readonly IVaultConnectionHandler _connectionHandler;

        internal Userpass(IVaultConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }
    }
}
