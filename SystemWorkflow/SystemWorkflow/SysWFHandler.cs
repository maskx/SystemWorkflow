using System.Activities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Routing;

namespace maskx.SystemWorkflow
{
    class SysWFHandler : HttpMessageHandler
    {
        readonly object _LockObj = new object();
        readonly RouteSetting _Setting;
        readonly ConcurrentDictionary<string, WorkflowInvoker> _InvokerCache = new ConcurrentDictionary<string, WorkflowInvoker>();
        public SysWFHandler(RouteSetting setting)
        {
            _Setting = setting;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                IHttpRouteData routeData = request.GetRouteData();
                Contract.Assert(routeData != null);
                if (!routeData.Values.TryGetValue(RouteConstants.SysWFPath, out object obj))
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "workflow Id not set in request");
                string id = obj.ToString();
                var invoker = GetWorkflowInvoker(id);
                if (invoker == null)
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "workflow Id not set in request");
                var content = request.Content;
                string jsonContent = content.ReadAsStringAsync().Result;
                dynamic b = WFJson.Parse(jsonContent);
                var rtv = invoker.Invoke(new Dictionary<string, object>() {
                            { "context",b },
                            { "data",b }
                        });
                return request.CreateResponse(HttpStatusCode.OK, (rtv["output"] as WFJson)._JToken);

            });
            WorkflowInvoker CreateWorkflowInvoker(string id)
            {
                WorkflowInvoker invoker = null;
                Activity activity = null;
                if (_Setting.GetStream != null)
                    activity = ActivityFactory.Create(id, _Setting.GetStream);
                else if (_Setting.GetFile != null)
                    activity = ActivityFactory.Create(id, _Setting.GetFile);
                if (activity == null)
                    return null;
                invoker = new WorkflowInvoker(activity);
                return invoker;
            }
            WorkflowInvoker GetWorkflowInvoker(string id)
            {
                if (_Setting.SafeMode)
                    return CreateWorkflowInvoker(id);
                if (_InvokerCache.TryGetValue(id, out WorkflowInvoker invoker))
                    return invoker;
                lock (_LockObj)
                {
                    if (!_InvokerCache.TryGetValue(id, out invoker))
                    {
                        invoker = CreateWorkflowInvoker(id);
                        _InvokerCache.TryAdd(id, invoker);
                    }
                }
                return invoker;
            }
        }
    }
}
