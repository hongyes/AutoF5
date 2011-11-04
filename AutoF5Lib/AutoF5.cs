using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace AutoF5Lib
{
    public static class AutoF5
    {
        const string ReturnMessage = "{{'serverTick': '{0}', 'type': {1} }}";
        public static long LastUpdateTime = System.DateTime.Now.Ticks;
        public static RefreshType RefreshType;
        private static List<string> fileExtensions;
        private static List<FileSystemWatcher> fsws;

        static AutoF5()
        {
            fileExtensions = new List<string>();
            fileExtensions.Add("*.cshtml");
            fileExtensions.Add("*.vbhtml");
            //fileExtensions.Add("*.cs");
            //fileExtensions.Add("*.vb");
            fileExtensions.Add("*.html");
            fileExtensions.Add("*.css");
            fileExtensions.Add("*.js");
            fileExtensions.Add("*.aspx");
            fileExtensions.Add("*.asax");
            //fileExtensions.Add("*.config");
            fileExtensions.Add("*.master");
            fileExtensions.Add("*.ascx");
            fileExtensions.Add("*.htm");
            //fileExtensions.Add("*.ashx");
            //fileExtensions.Add("*.svc");
            //fileExtensions.Add("*.dll");
            fsws = new List<FileSystemWatcher>();
        }

        public static void Start(HttpApplication context)
        {
            //DynamicModuleUtility.RegisterModule(typeof(AutoF5Module));

            string path = HostingEnvironment.ApplicationPhysicalPath;

            foreach (var ext in fileExtensions)
            {
                var fsw = new FileSystemWatcher(path);
                fsw.IncludeSubdirectories = true;
                fsw.NotifyFilter = NotifyFilters.LastWrite;
                if (ext == "*.css")
                    fsw.Changed += new FileSystemEventHandler(css_Changed);
                else
                    fsw.Changed += new FileSystemEventHandler(fsw_Changed);
                fsw.Filter = ext;
                fsw.EnableRaisingEvents = true;

                fsws.Add(fsw);
            }
        }

        public static void Stop()
        {
            foreach (var fsw in fsws)
            {
                fsw.EnableRaisingEvents = false;
                fsw.Dispose();
            }
            fsws = null;
        }

        static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            LastUpdateTime = System.DateTime.Now.Ticks;
            RefreshType = AutoF5Lib.RefreshType.Page;
        }

        static void css_Changed(object sender, FileSystemEventArgs e)
        {
            LastUpdateTime = System.DateTime.Now.Ticks;
            RefreshType = AutoF5Lib.RefreshType.Css;
        }

        internal static string ToJson()
        {
            return string.Format(
                ReturnMessage,
                AutoF5.LastUpdateTime.ToString(),
                (int)AutoF5.RefreshType);
        }
    }
}
