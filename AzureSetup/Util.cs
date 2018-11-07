using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;

namespace AzureSetup
{
    public static class Util
    {
        internal static Random _rng = new Random();

        public static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!";
            char[] chars = new char[passwordLength];

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[_rng.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static IAzure Auth()
        {
            CreateAzureAuthPropertiesInRegistry();
            var credentials = SdkContext.AzureCredentialsFactory.FromFile(Environment.GetEnvironmentVariable("AZURE_AUTH_LOCATION", EnvironmentVariableTarget.User));
            return Azure.Configure().WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic).Authenticate(credentials).WithDefaultSubscription();
        }

        private static void CreateAzureAuthPropertiesInRegistry()
        {
            using (PowerShell powerShellInstance = PowerShell.Create())
            {
                powerShellInstance.AddScript($@"[Environment]::SetEnvironmentVariable('AZURE_AUTH_LOCATION', '{AppDomain.CurrentDomain.BaseDirectory}azureauth.properties', 'User')");
                Collection<PSObject> output = powerShellInstance.Invoke();
            }
        }

        public static async Task<string> GetToken(string clientId, string clientSecret, string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }
    }
}
