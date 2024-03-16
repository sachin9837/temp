namespace EventCopilotBot.Services
{
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    public interface ISecretManager
    {
        Task<string> GetSecretAsync(string secretName);
    }

    public class SecretManager : ISecretManager
    {
        private readonly SecretClient _secretClient;
        private readonly Dictionary<string, string> _secretsCache = new Dictionary<string, string>();

        public SecretManager(IConfiguration configuration)
        {
            var keyVaultName = configuration["VaultConfig:KeyVaultName"];
            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net");
            _secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            if (_secretsCache.TryGetValue(secretName, out string cachedSecret))
            {
                return cachedSecret;
            }

            KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);

            _secretsCache[secretName] = secret.Value;

            return secret.Value;
        }
    }

}
