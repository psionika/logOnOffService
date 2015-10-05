using System;

namespace logOnOffService
{
    static class logOnOffInfo
    {
        public static DateTime dt = DateTime.Now;
        public static string NameComp = Environment.MachineName;
        public static string PublicIP = "127.0.0.1";
        public static string UserName = "";
        public static string actionCurrent = "Start";
    }
}
