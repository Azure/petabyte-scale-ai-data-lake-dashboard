// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class AdlsInsightClient
    {
        private string dataAtRestQueryUrlFormat =
            @"https://{0}.azuredatalakestore.net/StorageInsight/CustomerDataAtRest?starttime={1}&endtime={2}&api-version=2016-11-01";

        public async Task<long> GetDataAtRestInBytes(string accountName, string authToken)
        {
            var startDateTime = DateTime.Now.AddHours(-48);
            var endDateTime = DateTime.Now;

            var result = 0L;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var requestUrl = string.Format(dataAtRestQueryUrlFormat, accountName, 
                    Convert.ToString(startDateTime, CultureInfo.InvariantCulture), Convert.ToString(endDateTime, CultureInfo.InvariantCulture));
                var response = await client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Trace.TraceError("HTTP call failed. Response: {0}; response body: {1}.", response.ToString(), 
                        response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty);
                    throw new Exception(string.Format("Failed to get response from {0}; HTTP status code: {1}", 
                        dataAtRestQueryUrlFormat, response.StatusCode));
                }

                var responseList = JsonConvert.DeserializeObject<List<ExpandoObject>>(await response.Content.ReadAsStringAsync());

                IDictionary<string, object> latestEntry = null;
                foreach (var entry in responseList)
                {
                    if (latestEntry == null)
                    {
                        latestEntry = entry as IDictionary<string, object>;
                    }
                    else if (DateTime.Parse(latestEntry["utcDate"].ToString(), CultureInfo.InvariantCulture)
                        < DateTime.Parse((entry as IDictionary<string, object>)["utcDate"].ToString(), CultureInfo.InvariantCulture))
                    {
                        latestEntry = entry;
                    }
                }

                result = latestEntry != null ? Convert.ToInt64(latestEntry["totalBytes"]) : 0L;
            }

            return result;
        }

    }
}