using System;
using System.Data;
using System.IO;
using System.Reflection;

using NLog;

namespace logOnOffService
{
    class logOnOffBuffer
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static DataSet dataset1;

        static DataTable log()
        {
            var table = new DataTable("logBuffer");
            table.Columns.Add("dt", typeof(DateTime));
            table.Columns.Add("nameComp", typeof(string));
            table.Columns.Add("publicIP", typeof(string));
            table.Columns.Add("userName", typeof(string));
            table.Columns.Add("action", typeof(string));

            return table;
        }

        static string FilenameGet()
        {
            try
            {
                var x = Assembly.GetExecutingAssembly().Location;

                x = x.Remove(x.LastIndexOf("\\", StringComparison.CurrentCulture) + 1);

                x = x + "buffer.xml";

                return x;
            }
            catch (Exception msg)
            {
                logger.Error("ERROR BufferFilenameGet: " + msg);
                Environment.Exit(0);
                return "buffer.xml";
            }
        }

        public static DataSet load()
        {
            var ds = new DataSet("buffer");

            ds.Tables.Add(log());
            ds.Clear();

            try
            {
                var bufferFile = FilenameGet();

                if (!File.Exists(bufferFile)) return ds;

                ds.ReadXml(bufferFile);
            }
            catch (Exception msg)
            {
                logger.Error("ERROR loadBuffer: " + msg);
            }

            return ds;
        }

        public static void writeToFile()
        {
            try
            {
                var bufferFile = FilenameGet();

                dataset1.WriteXml(bufferFile, XmlWriteMode.IgnoreSchema);
            }
            catch (Exception msg)
            {
                logger.Error("ERROR saveBuffer: " + msg);
            }
        }

        public static void saveTo()
        {
            try
            {
                logger.Info("Save to buffer: " + logOnOffInfo.NameComp + ", " + logOnOffInfo.PublicIP
                + ", " + logOnOffInfo.UserName + ", " + logOnOffInfo.actionCurrent);

                var newRow = dataset1.Tables[0].NewRow();

                newRow["dt"] = DateTime.Now;
                newRow["nameComp"] = logOnOffInfo.NameComp;
                newRow["publicIP"] = logOnOffInfo.PublicIP;
                newRow["userName"] = logOnOffInfo.UserName;
                newRow["action"] = logOnOffInfo.actionCurrent;

                dataset1.Tables["logBuffer"].Rows.Add(newRow);
            }
            catch (Exception msg)
            {
                logger.Error("ERROR saveToBuffer: " + msg);
            }

            logOnOffBuffer.writeToFile();
        }

        public static void clear()
        {
            try
            {
                var bufferFile = FilenameGet();

                using (
                var ds = new DataSet("buffer"))
                {
                    ds.Clear();

                    ds.WriteXml(bufferFile, XmlWriteMode.IgnoreSchema);

                    logger.Trace("-------Buffer clear----------");
                }
            }
            catch (Exception msg)
            {
                logger.Error("ERROR clearBuffer: " + msg);
            }
        }
    }
}
