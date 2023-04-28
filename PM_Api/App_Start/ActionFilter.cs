using Newtonsoft.Json;
using PM_Api.Common;
using PM_Api.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace PM_Api.App_Start
{
    public class ActionFilter : ActionFilterAttribute
    {
        private string controllerName = "";
        private string actionName = "";
        private string RequestParm = "";
        private string IP = "";
        private const string Key = "action";
        private bool _IsDebugLog = true;

        private readonly string _operModul; // 操作模块
        private readonly string _operType; // 操作类型
        private readonly string _operDesc; // 操作说明

        public ActionFilter() {}

        public ActionFilter(string operModul, string operType, string operDesc)
        {
            _operModul = operModul;
            _operType = operType;
            _operDesc = operDesc;
        }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (_IsDebugLog)
            {
                Stopwatch stopWatch = new Stopwatch();

                actionContext.Request.Properties[Key] = stopWatch;

                IP = GetClientIpAddress(actionContext);
                //获取控制器
                controllerName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                //获取方法名
                actionName = actionContext.ActionDescriptor.ActionName;
                //获取请求参数
                RequestParm = JsonConvert.SerializeObject(actionContext.ActionArguments);

                var a = _operModul;
                var b = _operType;
                var c = _operDesc;

                stopWatch.Start();

                //进行Token验证
                var SecretKey = CheckSecretKey.ContrastSecretKey(RequestParm);
                if (SecretKey is bool && (bool)SecretKey == true)
                {
                    return;
                }
                else
                {
                    var errorResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(SecretKey), Encoding.UTF8, "application/json")
                    };
                    actionContext.Response = errorResponse;
                    return;
                }
            }

        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (_IsDebugLog)
            {
                Stopwatch stopWatch = actionExecutedContext.Request.Properties[Key] as Stopwatch;

                if (stopWatch != null)
                {
                    stopWatch.Stop();
                    string userId = HttpContext.Current.Request.Headers["userId"];
                    var StatusCode = actionExecutedContext.Response.StatusCode; ;
                    var Result = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                    var ResponseTime = (int)stopWatch.Elapsed.TotalMilliseconds;
                    log(userId, controllerName +"/" + actionName, RequestParm, Result, ResponseTime.ToString()+"ms", IP);
                }
            }
        }

        public string GetClientIpAddress(HttpActionContext actionContext)
        {
            string ipAddress = string.Empty;

            if (actionContext != null)
            {
                if (actionContext.Request.Headers.Contains("X-Forwarded-For"))
                {
                    ipAddress = actionContext.Request.Headers.GetValues("X-Forwarded-For").FirstOrDefault();
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    if (actionContext.Request.Properties.ContainsKey("MS_HttpContext"))
                    {
                        ipAddress = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                    }
                    else if (actionContext.Request.Properties.ContainsKey("MS_OwinContext"))
                    {
                        ipAddress = ((Microsoft.Owin.OwinContext)actionContext.Request.Properties["MS_OwinContext"]).Request.RemoteIpAddress;
                    }
                    else
                    {
                        ipAddress = actionContext.Request.Properties["MS_HttpRequestRemoteIpAddress"].ToString();
                    }
                }
            }
            return ipAddress;
        }

        public void log(string UserId, string Method, string RequestParm, string ResponseParm, string ResponseTime, string IP)
        {
            try
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                // 添加键值对
                dict.Add("UserId", UserId);
                dict.Add("Method", Method);
                dict.Add("RequestParm", RequestParm);
                dict.Add("ResponseParm", ResponseParm);
                dict.Add("ResponseTime", ResponseTime);
                dict.Add("IP", IP);
                dict.Add("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                SQLite.ExecuteNonQuery("INSERT INTO Log(UserId, Method, RequestParm, ResponseParm, ResponseTime, IP, CreateTime) VALUES(@UserId, @Method, @RequestParm, @ResponseParm, @ResponseTime, @IP, @CreateTime);", dict);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}