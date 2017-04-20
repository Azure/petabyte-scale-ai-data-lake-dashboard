// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using Microsoft.Azure;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AasClient
    {
        private static string SubscriptionId = CloudConfigurationManager.GetSetting("app:subscriptionId");
        private static string ResourceGroupName = CloudConfigurationManager.GetSetting("app:resourceGroupName");
        private static string AasServerName = CloudConfigurationManager.GetSetting("app:aasServerName");

        private string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        private string QueryFormat = @"providers/Microsoft.AnalysisServices/" +
            @"servers/{0}/providers/Microsoft.Insights/metrics?api-version=2016-06-01&" +
            @"$filter=(name.value eq 'CurrentUserSessions') and (aggregationType eq 'Average')" +
            @" and startTime eq {1} and endTime eq {2} and timeGrain eq duration'PT1M'";

        public async Task<int> GetActiveUserSessionCount(string authToken)
        {
            var startDateTime = DateTime.Now.ToUniversalTime().AddMinutes(-60);
            var startDateTimeString = startDateTime.ToString(DateTimeFormat);

            var endDateTime = DateTime.Now.ToUniversalTime();
            var endDateTimeString = endDateTime.ToString(DateTimeFormat);

            var query = string.Format(QueryFormat, AasServerName, startDateTimeString, endDateTimeString);
            query = Uri.EscapeUriString(query);

            ResourceManagerClient client = new ResourceManagerClient();
            var resultString = await client.Call(SubscriptionId, ResourceGroupName, query, authToken);

            var response = JsonConvert.DeserializeObject<ExpandoObject>(resultString);

            var dataObject = (response as IDictionary<string, object>)["value"];
            var resultList = (dataObject as IList<object>);

            if (resultList.Count < 1)
            {
                return 0;
            }

            var result = 0;
            var latestDateTime = startDateTime;
            foreach (var entry in (resultList.First() as IDictionary<string, object>)["data"] as IList<object>)
            {
                var entryDict = entry as IDictionary<string, object>;

                if (entryDict == null || !entryDict.ContainsKey("average") || !entryDict.ContainsKey("timeStamp"))
                {
                    continue;
                }

                int currentAvg = (int)Math.Ceiling(Double.Parse(entryDict["average"].ToString()));
                DateTime currentDateTime = DateTime.Parse(entryDict["timeStamp"].ToString());

                if (latestDateTime < currentDateTime)
                {
                    result = currentAvg;
                }   
            }

            return result;
        }
    }
}
