using Microsoft.VisualStudio.TestTools.UnitTesting;
using maskx.SystemWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using Owin;
using Microsoft.Owin.Hosting;

namespace maskx.SystemWorkflow.Tests
{
    [TestClass()]
    [TestCategory("WebApi")]
    public class WebApiTests
    {
        static string baseUrl = "http://localhost:3338";
        static void Configuration(IAppBuilder builder)
        {
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.MapSysWFServiceRoute("SysWF", "wf",
                new RouteSetting()
                {
                    SafeMode = false,
                    GetFile = (id) =>
                    {
                        return System.IO.Path.Combine(Environment.CurrentDirectory, "../../wf", id + ".xaml");
                    }
                });
            builder.UseWebApi(configuration);
        }
        static string Post(string query, object arg)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsJsonAsync(query, arg).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        static string InvokeWF(string id, object arg)
        {
            string tpl = baseUrl + "/wf/{0}";
            using (WebApp.Start(baseUrl, Configuration))
            {
                return Post(string.Format(tpl, id), arg);
            }
        }
    }
}