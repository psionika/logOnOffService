using System;
using System.Data;
using System.ServiceProcess;
using System.Reflection;
using System.Management;

using NLog;

namespace logOnOffService
{
    public partial class logOnOffService : ServiceBase
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        const int SERVICE_ACCEPT_PRESHUTDOWN = 0x100;
        const int SERVICE_CONTROL_PRESHUTDOWN = 0xf;

        public logOnOffService()
        {
            InitializeComponent();
            AcceptPreShutdown();
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("Start service");

            try
            {
                logOnOffSettingAction.ReadXml();
                logger.Trace("MySQL server - " + logOnOffSettings.logOnOffserverBD);

                checkBuffer();

                logOnOffInfo.UserName = "-No users-";
                logSend("Start computer");
            }
            catch (Exception msg)
            {
                logger.Error("ERROR Main module: " + msg);
            }

            while(true)
            {
                if (getUserName() != "")
                {
                    break;
                }
            }
        }

        protected override void OnStop()
        {
            logSend("Close manual");
        }

        protected override void OnShutdown()
        {
            logSend("Shutdown");
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == SERVICE_CONTROL_PRESHUTDOWN)
            {
                logSend("Shutdown");
            }
            else
                base.OnCustomCommand(command);
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch(powerStatus)
            {
                case PowerBroadcastStatus.Suspend:
                    logger.Trace("OnPowerEvent: Computer suspend");
                    logSend("Suspend");
                    break;
                case PowerBroadcastStatus.ResumeSuspend:
                    logger.Trace("OnPowerEvent: Computer resume");
                    logSend("Resume");
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    logOnOffInfo.UserName = getUserName();
                    logSend("logon");
                    break;
                case SessionChangeReason.SessionLogoff:
                    logSend("logoff");
                    break;
                case SessionChangeReason.SessionLock:
                    logOnOffInfo.UserName = getUserName();
                    logSend("lock");
                    break;
                case SessionChangeReason.SessionUnlock:
                    logOnOffInfo.UserName = getUserName();
                    logSend("unlock");
                    break;
            }

            base.OnSessionChange(changeDescription);
        }

        void logSend(string action)
        {
            logger.Info(action);

            var getPublicIP = new PublicIP();
            getPublicIP.Get();

            logOnOffInfo.actionCurrent = action;
            logOnOffInfo.dt = DateTime.Now;

            logOnOffBuffer.saveTo();

            if (logOnOffMySQL.Send()) logOnOffBuffer.clear();
        }

        void checkBuffer()
        {
            logger.Trace("--------Check buffer---------");
            logOnOffBuffer.dataset1 = logOnOffBuffer.load();

            if (logOnOffBuffer.dataset1.Tables[0].Rows.Count > 0)
            {
                logger.Info("Count buffer element - " + logOnOffBuffer.dataset1.Tables[0].Rows.Count);

                foreach (DataRow row in logOnOffBuffer.dataset1.Tables["logBuffer"].Rows)
                {
                    logger.Trace("----Send buffer element------");
                    logger.Trace("dt: " + row["dt"]+ " Computer name: " + row["nameComp"]
                               + " Public IP: " + row["publicIP"]
                               + " User name: " + row["userName"]
                               + " Action: " + row["action"]);

                    logOnOffInfo.dt = (DateTime)row["dt"];
                    logOnOffInfo.NameComp = (string)row["nameComp"];
                    logOnOffInfo.PublicIP = (string)row["publicIP"];
                    logOnOffInfo.UserName = (string)row["userName"];
                    logOnOffInfo.actionCurrent = (string)row["action"];

                    logOnOffMySQL.Send();
                }
            }
            else
            {
                logger.Trace("--------Buffer empty---------");
            }
        }

        public static string getUserName()
        {
            try
            {
                var x = "";

                var connectionOptions = new ConnectionOptions();

                var scope = new System.Management.ManagementScope("\\\\localhost", connectionOptions);
                var query = new System.Management.ObjectQuery("select * from Win32_ComputerSystem");

                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    var builder = new System.Text.StringBuilder();
                    builder.Append(x);

                    foreach (var row in searcher.Get())
                    {
                        builder.Append((row["UserName"].ToString() + " "));
                    }
                    x = builder.ToString();

                    return x;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        void AcceptPreShutdown()
        {
            if (Environment.OSVersion.Version.Major == 5)
                return; // XP & 2003 does not support this.

            var acceptedCommandsFieldInfo = typeof(ServiceBase).GetField("acceptedCommands", BindingFlags.Instance | BindingFlags.NonPublic);
            if (acceptedCommandsFieldInfo == null)
                return;

            var value = (int)acceptedCommandsFieldInfo.GetValue(this);
            acceptedCommandsFieldInfo.SetValue(this, value | SERVICE_ACCEPT_PRESHUTDOWN);
        }
    }
}
