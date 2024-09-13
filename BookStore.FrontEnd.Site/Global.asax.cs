using AutoMapper;
using BookStore.FrontEnd.Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BookStore.FrontEnd.Site
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static IMapper _mapper;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //AutoMapper配置
            var config = new MapperConfiguration(cfg =>
            {
                //使用MappingProfile來設定對應
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }
    }
}
