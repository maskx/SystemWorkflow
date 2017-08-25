using System;
using System.IO;
using System.Net.Http;

namespace maskx.SystemWorkflow
{
    /// <summary>
    /// Setting of SystemWorkflow Service Route
    /// </summary>
    public class RouteSetting
    {
        /// <summary>
        /// In SafeMode,will always create the new instance of every workflow
        /// The default value is true
        /// </summary>
        public bool SafeMode { get; set; } = true;
        /// <summary>
        /// the method get workflow define file(*.xaml file)
        /// </summary>
        public Func<string, string> GetFile { get; set; }
        /// <summary>
        /// The method get the stream defined workflow
        /// Use this property when the workflow define file save in database
        /// When set this, GetFile property will not work
        /// </summary>
        public Func<string, Stream> GetStream { get; set; }
        public Func<HttpRequestMessage,WFJson> GetContext { get; set; }
    }
}
