using NexusNetworkCloud.CsharpVault;
using NexusNetworkCloud.CsharpVault.Vault;

namespace CsharpVaultTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            VaultConnectionConfiguration config = new()
            {
                VaultAddress = "https://vault.nexus-network.cloud",
                Port = 443
            };

            using (VaultClient client = new VaultClient(config))
            {
                client.KV2.SetMount("secret-store");
                var kv2ReadResult = await client.KV2.ReadSecretAsync("phillips-hue");
            }
        }
    }
}