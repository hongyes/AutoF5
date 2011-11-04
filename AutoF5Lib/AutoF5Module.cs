using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Threading;

namespace AutoF5Lib
{
    public class AutoF5Module : IHttpModule
    {
        private static bool isMonitorInited;
        private static string scriptCode;
        private static string ScriptCode
        {
            get
            {
                if (string.IsNullOrEmpty(scriptCode))
                {
                    var page = new Page();
                    string scriptPath =
                        page.ClientScript.GetWebResourceUrl(
                            typeof(AutoF5Module),
                            "AutoF5Lib.autoF5.js");
                    scriptCode = string.Format(
                        @"<script src=""{0}"" type=""text/javascript""></script>", 
                        scriptPath);
                }
                return scriptCode;
            }
        }

            
        private HttpApplication application;
        public void Dispose()
        {
            //AutoF5.Stop();
        }

        public void Init(HttpApplication context)
        {
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                application = context;
                EnsureFirstInit();

                context.BeginRequest += new EventHandler(context_BeginRequest);
                context.EndRequest += new EventHandler(context_EndRequest);
            }
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HandleCheckRequest();
        }

        private bool HandleCheckRequest()
        {
            var url = application.Request.Url;
            if (url != null
                && url.AbsolutePath.EndsWith("/_autof5_check", StringComparison.InvariantCultureIgnoreCase))
            {
                string tickStr = application.Request["tick"];
                long tick;
                if (long.TryParse(tickStr, out tick))
                {
                    if (tick == 0)
                        return Content(AutoF5.ToJson());

                    for (int i = 0; i < 50; i++)
                    {
                        if (tick < AutoF5.LastUpdateTime)
                            return Content(AutoF5.ToJson());
                        else
                            Thread.Sleep(400);
                    }

                }

                return Content("");
            }
            else
                return false;
        }

        private bool Content(string content)
        {
            try
            {
                application.Response.ContentType = "text/plain";
                application.Response.Write(content);
                application.Response.End();
                return true;
            }
            catch{
                return false;
            }
        }

        private void EnsureFirstInit()
        {
            try
            {
                if (!isMonitorInited)
                    lock (this)
                        if (!isMonitorInited)
                        {
                            FirstInit();
                            isMonitorInited = true;
                        }
            }
            catch { }
        }

        private void FirstInit()
        {
            AutoF5.Start(application);
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication context = ((HttpApplication)sender);
            if (Utility.ArrayContains(context.Request.AcceptTypes, "text/html")
               && (context.Request["X-Requested-With"] != "XMLHttpRequest"))
            {
                if ((context.Response.ContentType.ToLower() == "text/html"))
                {
                    context.Response.Write(
                        ScriptCode.ToCharArray(), 
                        0,
                        ScriptCode.Length);
                }
            }
            //context.EndRequest -= new EventHandler(context_EndRequest);
        }
    }
}
