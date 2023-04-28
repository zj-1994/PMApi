using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace PM_Api.Common
{
    public class SQLite
    {
        /// <summary>        
        /// 获得连接对象        
        /// </summary>        
        /// <returns>SQLiteConnection</returns>        
        public static SQLiteConnection GetSQLiteConnection()
        {
            //Sqlite数据库地址
            string str = System.AppDomain.CurrentDomain.BaseDirectory + "\\DataBass\\LogDB.db";
            //SQLiteConnection.CreateFile(str); // 创建文件
            var con = new SQLiteConnection("Data Source=" + str);
            return con;
        }

        /// <summary>
        /// 准备操作命令参数
        /// </summary>
        /// <param name="cmd">SQLiteCommand</param>
        /// <param name="conn">SQLiteConnection</param>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">参数数组</param>
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, Dictionary<String, String> data)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (data != null && data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    cmd.Parameters.AddWithValue(val.Key, val.Value);
                }
            }
        }

        private static void ListPrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, Dictionary<String, String> data)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            if (data != null && data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    cmd.Parameters.AddWithValue(val.Key, val.Value);
                }
            }
            cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// 查询，返回DataTable
        /// </summary>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">参数数组</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string cmdText, Dictionary<string, string> data)
        {
            var dt = new DataTable();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                var command = new SQLiteCommand();
                PrepareCommand(command, connection, cmdText, data);
                SQLiteDataReader reader = command.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }

        /// <summary>
        /// 查询，返回DataSet
        /// </summary>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">参数数组</param>
        /// <returns>DataSet</returns>
        public static DataSet ExecuteDataset(string cmdText, Dictionary<string, string> data)
        {
            var ds = new DataSet();
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                var command = new SQLiteCommand();
                PrepareCommand(command, connection, cmdText, data);
                var da = new SQLiteDataAdapter(command);
                da.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 执行数据库操作
        /// </summary>
        /// <param name="cmdText">Sql命令文本</param>
        /// <param name="data">传入的参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, Dictionary<string, string> data)
        {
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                var command = new SQLiteCommand();
                PrepareCommand(command, connection, cmdText, data);
                return command.ExecuteNonQuery();
            }
        }

        public static int ExecuteNonQuery(string cmdText, List<Dictionary<string, string>> data)
        {
            using (SQLiteConnection connection = GetSQLiteConnection())
            {
                var command = new SQLiteCommand();
                for (int i = 0; i < data.Count; i++)
                {
                    ListPrepareCommand(command, connection, cmdText, data[i]);
                }
                return 1;
            }
        }
    }
}