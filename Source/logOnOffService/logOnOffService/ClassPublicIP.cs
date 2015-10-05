using System;
using System.IO;
using System.Net;

using NLog;
using System.Text;

namespace logOnOffService
{
    public class PublicIP
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void Get()
        {
            try
            {
                if (logOnOffInfo.PublicIP != "127.0.0.1")
                {
                    return;
                }

                logOnOffInfo.PublicIP = "127.0.0.1";

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Proxy = null;

                    var downloadString = client.DownloadString(new Uri("http://yandex.ru/internet/"));

                    var first = downloadString.IndexOf("<strong>IP-адрес</strong>: ", StringComparison.Ordinal) + 27;
                    var last = downloadString.IndexOf("<strong>Регион по IP-адресу</strong>", StringComparison.Ordinal);
                    downloadString = downloadString.Substring(first, last - first);
                    logOnOffInfo.PublicIP = downloadString;
                }
            }
            catch (Exception msg)
            {
                logger.Error("Error: (PublicIP.Get) " + msg);
            }
        }
    }
}
