// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AiDataLakeDevOpsDashboard
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/External/bootstrap.css",
                "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/Content/scripts").Include(
                "~/Scripts/External/jquery-1.10.2.js",
                "~/Scripts/Animators.js",
                "~/Scripts/Connectors.js",
                "~/Scripts/Formatters.js"));
        }
    }
}
