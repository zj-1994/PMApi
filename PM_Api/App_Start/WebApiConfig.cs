using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PM_Api.App_Start;

namespace PM_Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "Views", action = "Login", id = RouteParameter.Optional }
                //defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
