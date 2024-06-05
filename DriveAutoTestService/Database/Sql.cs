using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAutoTestService.Database
{
    public static class Sql
    {
        static MySqlConnection con;
        public static void Connect()
        {
            string connetStr = "server=md1u34qc;port=3306;user=root;password=rdst123456; database=labnow_dev;";
            con = new MySqlConnection(connetStr);
            con.Open();
        }
        public static void Disconnect()
        {
            con.Close();
        }
        public static void Execute(string sql)
        {
            Connect();
            MySqlCommand sqlCommand = new MySqlCommand(sql, con);
            int num = sqlCommand.ExecuteNonQuery();
            Disconnect();
        }
    }
}
