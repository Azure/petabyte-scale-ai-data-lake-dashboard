// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard.Controllers
{
    using Microsoft.Azure;
    using AiDataLakeDevOpsDashboard.Clients;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        // App config settings
        private readonly string sqlDbQueryTableName = CloudConfigurationManager.GetSetting("app:sqlDbQueryTableName");
        private readonly string sqlDwQueryTableName = CloudConfigurationManager.GetSetting("app:sqlDwQueryTableName");

        private readonly string adlaAccountName = CloudConfigurationManager.GetSetting("app:adlaAccountName");
        private readonly string adlsAccountName = CloudConfigurationManager.GetSetting("app:adlsAccountName");

        // Secret settings
        private readonly string AadAppKey = CloudConfigurationManager.GetSetting("AadAppKey");
        private readonly string SqlServerDbConnectionString = CloudConfigurationManager.GetSetting("SqlServerDbConnectionString");
        private readonly string SqlServerDwConnectionString = CloudConfigurationManager.GetSetting("SqlServerDwConnectionString");

        public ActionResult Index()
        {
            return View("Default");
        }

        public ActionResult Default()
        {
            return View("Default");
        }

        public async Task<long> GetAdlsStorage()
        {
            try
            {
                AzureADClient adClient = new AzureADClient();
                var token = await adClient.GetToken(AadAppKey, "https://management.azure.com/");

                AdlsInsightClient client = new AdlsInsightClient();
                return await client.GetDataAtRestInBytes(adlsAccountName, token);
            }
            catch (Exception e)
            {
                Trace.TraceError("HomeController.GetAdlsStorage failed with the following exception: " + e.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<string> GetAdlaJobCount()
        {
            AdlaManagementClient client = new AdlaManagementClient();
            try
            {
                return await client.GetActiveJobCount(adlaAccountName);
            }
            catch (Exception e)
            {
                Trace.TraceError("HomeController.GetAdlaJobCount failed with the following exception: " + e.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<int> GetSqlDbActiveQueryCount()
        {
            try
            {
                SqlServerClient client = new SqlServerClient();
                return await client.GetActiveQueryCount(SqlServerDbConnectionString, sqlDbQueryTableName);
            }
            catch (Exception e)
            {
                Trace.TraceError("HomeController.GetSqlDbActiveQueryCount failed with the following exception: " + e.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<int> GetSqlDwActiveQueryCount()
        {
            try
            {
                SqlServerClient client = new SqlServerClient();
                return await client.GetActiveQueryCount(SqlServerDwConnectionString, sqlDwQueryTableName);
            }
            catch (Exception e)
            {
                Trace.TraceError("HomeController.GetSqlDwActiveQueryCount failed with the following exception: " + e.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        public async Task<int> GetAasUserSessionCount()
        {
            try
            {
                AzureADClient adClient = new AzureADClient();
                var authToken = await adClient.GetToken(AadAppKey, "https://management.azure.com/");

                var client = new AasClient();
                return await client.GetActiveUserSessionCount(authToken);
            }
            catch (Exception e)
            {
                Trace.TraceError("HomeController.GetAasUserSessionCount failed with the following exception: " + e.ToString());
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
    }
}