using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace maskx.SystemWorkflow
{
    class RouteConstants
    {
        public static readonly string SysWFPath = "id";
        /// <summary>
        /// Wildcard route template for the SysWF path route variable.
        /// </summary>
        public static readonly string SysWFPathTemplate = "{" + SysWFPath + "}";
    }
}
