using System;
using System.Web;
using License.Manager.Core;

namespace License.Manager
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal)
                ServiceStack.MiniProfiler.Profiler.Start();
        }

        protected void Application_EndRequest(object src, EventArgs e)
        {
            ServiceStack.MiniProfiler.Profiler.Stop();
        }
    }
}