using System;

using MySql.Data.MySqlClient;
using NLog;

namespace logOnOffService
{
    class logOnOffMySQL
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool Send()
        {
            var cs = @"server=" + logOnOffSettings.logOnOffserverBD +
                     @";userid=" + logOnOffSettings.logOnOffuserBD +
                     @";password=" + logOnOffSettings.logOnOffpasswordBD +
                     @";database=" + logOnOffSettings.logOnOffnameBD +
                     @";Charset=utf8";

            MySqlConnection conn = null;

            try
            {
                using (conn = new MySqlConnection(cs))
                {
                    conn.Open();

                    using (var cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO log(dt, NameComp, PublicIP, UserName, Action)" +
                                          " VALUES(@dt, @NameComp, @PublicIP, @UserName, @Action)";
                        cmd.Prepare();

                        cmd.Parameters.AddWithValue("@dt", logOnOffInfo.dt);
                        cmd.Parameters.AddWithValue("@NameComp", logOnOffInfo.NameComp);
                        cmd.Parameters.AddWithValue("@PublicIP", logOnOffInfo.PublicIP);
                        cmd.Parameters.AddWithValue("@UserName", logOnOffInfo.UserName);
                        cmd.Parameters.AddWithValue("@Action", logOnOffInfo.actionCurrent);
                        cmd.ExecuteNonQuery();
                    }
                }

                logger.Info("Send to database complete!");

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("ERROR Send to MySQL: {0}", ex.ToString());

                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
