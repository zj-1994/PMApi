using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PM_Api.Models
{
    public class SQLParam
    {
        /// <summary>
        /// SQl 语句
        /// </summary>
        public String SQLStringList { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public List<SqlParameter> Param { get; set; }
    }
}