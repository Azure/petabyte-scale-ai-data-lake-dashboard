// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Clients
{
    using Microsoft.Azure;
    using Microsoft.Azure.Management.DataLake.Analytics;
    using Microsoft.Azure.Management.DataLake.Analytics.Models;
    using Microsoft.Rest.Azure.Authentication;
    using Microsoft.Rest.Azure.OData;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class AdlaManagementClient
    {
        private string AadTenant = CloudConfigurationManager.GetSetting("aad:Tenant");
        private string ApplicationId = CloudConfigurationManager.GetSetting("aad:ClientId");

        private string ApplicationKey = CloudConfigurationManager.GetSetting("AadAppKey");

        public async Task<string> GetActiveJobCount(string accountName)
        {
            Expression<Func<JobInformation, bool>> filterExpression = (JobInformation jobInfo) => jobInfo.State == JobState.Running;
            var query = new ODataQuery<JobInformation>(filterExpression);

            var client = await CreateJobClient();

            var page = await client.Job.ListAsync(accountName, query);
            if (page.NextPageLink != null)
            {
                return string.Format("{0}+", page.Count());
            }
            else
            {
                return string.Format("{0}", page.Count());
            }
        }

        private async Task<DataLakeAnalyticsJobManagementClient> CreateJobClient()
        {
            var credentials = await ApplicationTokenProvider.LoginSilentAsync(AadTenant, ApplicationId, ApplicationKey);
            return new DataLakeAnalyticsJobManagementClient(credentials);
        }
    }
}
