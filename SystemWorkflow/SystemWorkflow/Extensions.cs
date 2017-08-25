using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace maskx.SystemWorkflow
{
    public static class Extensions
    {
        /// <summary>
        /// Map the SysWF Service Route
        /// use this method when the workflow define file save in disk
        /// </summary>
        /// <param name="configuration">HttpConfiguration</param>
        /// <param name="routeName">The name of the route to map.</param>
        /// <param name="routePrefix">The Prefix of the route to map.</param>
        /// <param name="setting">Setting of SysWF Service Route</param>
        /// <returns>A reference to the mapped route</returns>
        public static IHttpRoute MapSysWFServiceRoute(
            this HttpConfiguration configuration,
            string routeName,
            string routePrefix,
            RouteSetting setting)
        {
            return configuration.Routes.MapHttpRoute(
               routeName,
               string.IsNullOrEmpty(routePrefix) ? RouteConstants.SysWFPathTemplate : routePrefix + '/' + RouteConstants.SysWFPathTemplate,
               null,
               null,
               new SysWFHandler(setting)
           );
        }

    }
}
