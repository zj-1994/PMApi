using Newtonsoft.Json;
using PM_Api.Common;
using PM_Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace PM_Api.Controllers
{
    public class UserController : ApiController
    {
        private static HttpSessionState _session = HttpContext.Current.Session;

        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="JsonParm">json对象 </param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UserLogin")]
        public IHttpActionResult UserLogin([FromBody] UserLogin Parm)
        {
            try
            {
                var Param = new SqlParameter[]
                {
                    new SqlParameter("@Account", Parm.Account),
                    new SqlParameter("@Password", Parm.Password)
                };
                string SQLString = "SELECT GID,Account,REPLACE(NEWID(),'-','') AS SignToken FROM SYS_User   where Account = @Account and Password = @Password";
                var ds = SqlHelp.SqlGetDataSet(SQLString, Param);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    var LoginUserId = ds.Tables[0].Rows[0]["GID"].ToString();
                    var Account = ds.Tables[0].Rows[0]["Account"].ToString();
                    var SignToken = ds.Tables[0].Rows[0]["SignToken"].ToString();
                    CommonMethods.SetCacheRelativeTime(LoginUserId, SignToken, 1800);
                    return Ok(new { code = 200, msg = "登录成功", data = new { LoginUserId = LoginUserId, LoginAccount = Account, SignToken = SignToken, } });
                }
                else
                {
                    return Ok(new { code = 500, msg = "登录失败！账号或密码错误" });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = "请求失败:" + ex.Message });
            }

        }

        #endregion
    }
}
