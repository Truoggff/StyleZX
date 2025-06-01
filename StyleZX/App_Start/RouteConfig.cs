using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StyleZX
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            ///*
            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new
                {
                    controller = "Admin",
                    action = "Manage",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "StyleZX.Controllers" }
            );
            //*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "StyleZX.Controllers" }
            );
        }
    }
}
