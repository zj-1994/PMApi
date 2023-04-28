using Newtonsoft.Json;
using PM_Api.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace PM_Api.Common
{
    public class SqlHelp
    {
        static string ConntinonString = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString.ToString();

        #region GetSql---------返回Sql语句
        /// <summary>
        /// 返回Sql语句
        /// </summary>
        /// <param name="SqlName"></param>
        /// <returns></returns>
        public static string GetSql(string SqlName)
        {
            string SQLString = "select * from Admin_Sql where SqlName = @SqlName";
            using (SqlConnection connection = new SqlConnection(ConntinonString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = 60;
                    command.SelectCommand.Parameters.Add(new SqlParameter("@SqlName", SqlName));
                    command.Fill(ds, "ds");
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        return ds.Tables[0].Rows[0]["SqlContent"].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
        }
        #endregion

        #region SelectSql---------返回Sql语句
        /// <summary>
        /// 返回Sql语句
        /// </summary>
        /// <param name="SqlName"></param>
        /// <returns></returns>
        public static Dictionary<string,string> SelectSql(string SqlName)
        {
            Dictionary<string, string> DicScore = new Dictionary<string, string>();
            string SQLString = "select * from Admin_Sql where SqlName = @SqlName";
            using (SqlConnection connection = new SqlConnection(ConntinonString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = 60;
                    command.SelectCommand.Parameters.Add(new SqlParameter("@SqlName", SqlName));
                    command.Fill(ds, "ds");
                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        DicScore.Add("SqlContent", ds.Tables[0].Rows[0]["SqlContent"].ToString());
                        DicScore.Add("TableName", ds.Tables[0].Rows[0]["TableName"].ToString());
                        return DicScore;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
        }
        #endregion

        #region SqlGetDataSet-------执行带参数的SQL语句，返回DataSet

        /// <summary>
        /// 执行带参数的SQL语句，返回DataSet
        /// </summary>
        /// <param name="SQL">Sql语句</param>
        /// <param name="Param">参数</param>
        /// <returns></returns>
        public static DataSet SqlGetDataSet(string SQL, params SqlParameter[] Param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConntinonString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(Param);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region SqlGetDataTable-------执行带参数的SQL语句，返回DataTable

        /// <summary>
        /// 执行带参数的SQL语句，返回DataSet
        /// </summary>
        /// <param name="SQL">Sql语句</param>
        /// <param name="Param">参数</param>
        /// <returns></returns>
        public static DataTable SqlGetDataTable(string SQL, params SqlParameter[] Param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConntinonString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = SQL;
                        cmd.Parameters.AddRange(Param);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region  执行多条/单条查询SQL语句，返回json
        /// <summary>
        /// 执行多条/单条查询SQL语句，返回json
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public static string ExecuteQuerySqlTran(List<SQLParam> paras)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(ConntinonString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        for (int n = 0; n < paras.Count; n++)//循环传入的sql语句  
                        {
                            cmd.CommandText = paras[n].SQLStringList; //设置执行命令的sql语句  
                            cmd.Parameters.AddRange(paras[n].Param.ToArray());
                            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            cmd.Parameters.Clear();
                            ds.Tables.Add(dt);
                        }
                        return DataSetToJson(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                return "{\"code\":500,\"msg\":\"请求失败！" + ex.Message + "\"}";
            }
        }
        #endregion

        #region  执行分页查询SQL语句，返回json
        /// <summary>
        /// 执行分页查询SQL语句，返回json
        /// </summary>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public static string ExecuteQuerySqlTranPage(List<SQLParam> paras)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection conn = new SqlConnection(ConntinonString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        for (int n = 0; n < paras.Count; n++)//循环传入的sql语句  
                        {
                            cmd.CommandText = paras[n].SQLStringList; //设置执行命令的sql语句  
                            cmd.Parameters.AddRange(paras[n].Param.ToArray());
                            SqlDataAdapter da = new SqlDataAdapter(cmd); ;
                            da.Fill(ds);
                            cmd.Parameters.Clear();
                        }
                        return DataSetToJson_Page(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                return "{\"code\":500,\"msg\":\"请求失败！" + ex.Message + "\"}";
            }
        }
        #endregion

        #region 执行多条增删改SQL语句，实现数据库事务。
        /// <summary>
        /// 执行多条增删改SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static string ExecuteOperationSqlTran(List<SQLParam> paras)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConntinonString))
            {
                sqlCon.Open(); //打开数据库连接  
                SqlCommand cmd = new SqlCommand(); //创建SqlCommand命令  
                cmd.Connection = sqlCon; //设置命令连接  
                SqlTransaction tx = sqlCon.BeginTransaction();//开始事务  
                cmd.Transaction = tx;//设置执行命令的事务  
                try
                {
                    int count = 0;//定义int类型变量，存放该函数返回值  
                    for (int n = 0; n < paras.Count; n++)//循环传入的sql语句  
                    {
                        cmd.CommandText = paras[n].SQLStringList; //设置执行命令的sql语句  
                        cmd.Parameters.AddRange(paras[n].Param.ToArray());
                        count += cmd.ExecuteNonQuery(); //调用执行增删改sql语句的函数ExecuteNonQuery(),执行sql语句  
                        cmd.Parameters.Clear();
                    }
                    tx.Commit();//提交事务  
                    return "{\"code\":200,\"msg\":\"操作成功!受影响行数(" + count + ")\"}";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    return "{\"code\":500,\"msg\":\"请求失败！" + ex.Message + "\"}";
                }
                finally
                {
                    sqlCon.Close();
                }
            }
        }
        #endregion

        #region DataSet转json
        /// <summary>
        /// 返回查询数据json
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToJson(DataSet ds)
        {
            if (ds == null || ds.Tables.Count <= 0 )
            {
                //如果查询到的数据为空则返回标记
                return "{\"code\":404,\"msg\":\"请求成功！无数据！\",\"data\":[]}";
            }
            else
            {
                if (ds.Tables.Count == 1)
                {
                    var sb = new StringBuilder();
                    sb.Append("{\"code\":200,\"msg\":\"请求成功！\",");
                    sb.Append(string.Format("\"{0}\":", "data"));
                    string JsonString = JsonConvert.SerializeObject(ds.Tables[0]);
                    sb.Append(JsonString);
                    sb.Append("}");
                    return sb.ToString();
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append("{\"code\":200,\"msg\":\"请求成功！\",\"data\":{");
                    foreach (DataTable dt in ds.Tables)
                    {
                        sb.Append(string.Format("\"{0}\":", dt.TableName));
                        string JsonString = JsonConvert.SerializeObject(dt);
                        sb.Append(JsonString);
                        sb.Append(",");
                    }
                    sb.Remove(sb.ToString().LastIndexOf(','), 1);
                    sb.Append("}}");
                    return sb.ToString();
                }
            }
        }
        #endregion

        #region DataSet转json
        /// <summary>
        /// 返回查询数据json
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static string DataSetToJson(DataSet ds,string TableName)
        {
            if (ds == null || ds.Tables.Count <= 0)
            {
                //如果查询到的数据为空则返回标记
                return "{\"code\":404,\"msg\":\"请求成功！无数据！\",\"data\":[]}";
            }
            else
            {
                if (ds.Tables.Count == 1)
                {
                    var sb = new StringBuilder();
                    sb.Append("{\"code\":200,\"msg\":\"请求成功！\",");
                    sb.Append(string.Format("\"{0}\":", "data"));
                    string JsonString = JsonConvert.SerializeObject(ds.Tables[0]);
                    sb.Append(JsonString);
                    sb.Append("}");
                    return sb.ToString();
                }
                else
                {
                    string[] sArray = TableName.Split(',');

                    var sb = new StringBuilder();
                    sb.Append("{\"code\":200,\"msg\":\"请求成功！\",\"data\":{");
                    foreach (DataTable dt in ds.Tables)
                    {
                        int index = ds.Tables.IndexOf(dt); //index 为索引值
                        sb.Append(string.Format("\"{0}\":", sArray[index]));
                        string JsonString = JsonConvert.SerializeObject(dt);
                        sb.Append(JsonString);
                        sb.Append(",");
                    }
                    sb.Remove(sb.ToString().LastIndexOf(','), 1);
                    sb.Append("}}");
                    return sb.ToString();
                }
            }
        }
        #endregion

        #region DataSet转分页数据json
        /// <summary>
        /// 返回分页数据json
        /// </summary>
        /// <param name="ds">dataset数据集</param>
        /// <returns>json格式的字符串</returns>
        public static string DataSetToJson_Page(DataSet ds)
        {
            StringBuilder sb = new StringBuilder();
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                //如果查询到的数据为空则返回标记
                return "{\"code\":200,\"msg\":\"未查询到数据！\",\"count\":0,\"data\":[]}";
            }
            else
            {
                sb.Append("{\"code\":200,\"msg\":\"请求成功！\",\"count\":\"" + ds.Tables[1].Rows[0][0] + "\",\"data\":");
                string jsonString = JsonConvert.SerializeObject(ds.Tables[0]);
                sb.Append(jsonString);
                sb.Append("}");
                return sb.ToString();
            }
        }
        #endregion

        #region DataSet绑定参数
        /// <summary>
        /// DataSet绑定参数
        /// </summary>
        /// <param name="ds">dataset数据集</param>
        /// <returns></returns>
        public static List<SQLParam> ParamAdd(DataSet ds)
        {
            List<SQLParam> SQLParamList = new List<SQLParam>();
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                string TableName = ds.Tables[i].TableName;
                string sql = SqlHelp.GetSql(TableName);
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {
                    SQLParam SQLParam = new SQLParam();
                    List<SqlParameter> paramList = new List<SqlParameter>();
                    for (int k = 0; k < ds.Tables[i].Columns.Count; k++)
                    {
                        var key = ds.Tables[i].Columns[k].ToString();
                        var value = ds.Tables[i].Rows[j]["" + key + ""].ToString();
                        SqlParameter param = new SqlParameter("@" + key, value);
                        paramList.Add(param);
                    }
                    SQLParam.SQLStringList = sql;
                    SQLParam.Param = paramList;
                    SQLParamList.Add(SQLParam);
                }
            }
            return SQLParamList;
        }
        #endregion

       
    }
}