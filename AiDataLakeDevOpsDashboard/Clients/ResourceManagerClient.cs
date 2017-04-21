// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using Microsoft.Azure;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class ResourceManagerClient
    {
        private string AzureRmBaseUrl = CloudConfigurationManager.GetSetting("app:azureRmBaseUrl");

        public async Task<string> Call(string subscriptionId, string resourceGroupName, string query, string authToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = string.Format("{0}/subscriptions/{1}/resourcegroups/{2}/{3}", AzureRmBaseUrl, subscriptionId, resourceGroupName, query);
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Trace.TraceError("HTTP call failed. Response: {0}; response body: {1}.", response.ToString(),
                        response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty);
                    throw new Exception("Failed to get response. " + response.StatusCode);
                }

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}