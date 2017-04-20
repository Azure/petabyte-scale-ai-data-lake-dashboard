// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using Microsoft.Azure;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Globalization;
    using System.Threading.Tasks;

    public class AzureADClient
    {
        private static string aadInstance = CloudConfigurationManager.GetSetting("aad:Instance");
        private static string tenant = CloudConfigurationManager.GetSetting("aad:Tenant");
        private static string clientId = CloudConfigurationManager.GetSetting("aad:ClientId");

        public async Task<string> GetToken(string appKey, string resourceId)
        {
            var clientCredential = new ClientCredential(clientId, appKey);

            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
            var authContext = new AuthenticationContext(authority);

            // ADAL includes an in memory cache, so this call will only send a message to the server if the cached token is expired.
            var result = await authContext.AcquireTokenAsync(resourceId, clientCredential);

            return result.AccessToken;
        }
    }
}