using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

using NLog;
using Microsoft.Win32;

namespace logOnOffService
{
    static class Program
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            

            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);

                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                using (var service1 = new logOnOffService())
                {
                    ServiceBase.Run(service1);
                }
            }
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error(((Exception)e.ExceptionObject).Message + ((Exception)e.ExceptionObject).InnerException.Message);
        }
    }
}
