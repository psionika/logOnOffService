using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Soap;

using NLog;

namespace logOnOffService
{
    static class logOnOffSettings
    {
        public static string logOnOffserverBD = @"127.0.0.1";
        public static string logOnOffnameBD = @"log";
        public static string logOnOffuserBD = @"user";
        public static string logOnOffpasswordBD = @"password";
    }

    static class logOnOffSettingAction
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        static string Filename = FilenameGet();

        static string FilenameGet()
        {
            var x = Assembly.GetExecutingAssembly().Location;

            x = x.Remove(x.LastIndexOf("\\", StringComparison.CurrentCulture) + 1);

            x = x + "settings.xml";

            return x;
        }

        public static void ReadXml()
        {
            try
            {
                var staticClass = typeof(logOnOffSettings);

                if (!File.Exists(Filename)) return;

                var fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);

                using (Stream f = File.Open(Filename, FileMode.Open))
                {
                    var formatter = new SoapFormatter();
                    var a = formatter.Deserialize(f) as object[,];
                    f.Close();
                    if (a != null && a.GetLength(0) != fields.Length) return;
                    var i = 0;
                    foreach (var field in fields)
                    {
                        if (a != null && field.Name == (a[i, 0] as string))
                        {
                            if (a[i, 1] != null)
                                field.SetValue(null, a[i, 1]);
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Trace("ERROR Send to MySQL: {0}", ex.ToString());
            }

        }
    }
}
