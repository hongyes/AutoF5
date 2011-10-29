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
        public static long LastUpdateTime = System.DateTime.Now.Ticks;
        private static List<string> fileExtensions;
        private static List<FileSystemWatcher> fsws;

        static AutoF5()
        {
            fileExtensions = new List<string>();
            fileExtensions.Add("*.cshtml");
            fileExtensions.Add("*.vbhtml");
            fileExtensions.Add("*.cs");
            fileExtensions.Add("*.vb");
            fileExtensions.Add("*.html");
            fileExtensions.Add("*.css");
            fileExtensions.Add("*.js");
            fileExtensions.Add("*.aspx");
            fileExtensions.Add("*.asax");
            fileExtensions.Add("*.config");
            fileExtensions.Add("*.master");
            fileExtensions.Add("*.ascx");
            fileExtensions.Add("*.htm");
            fileExtensions.Add("*.ashx");
            fileExtensions.Add("*.svc");
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
        }
    }
}
