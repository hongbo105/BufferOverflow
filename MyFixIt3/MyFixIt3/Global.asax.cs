using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyFixIt3.Persistence;
using Autofac;

namespace MyFixIt3
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var container = new ContainerBuilder();

            container.RegisterType<FixItQueueManager>().As<IFixItQueueManager>();
            container.RegisterType<PhotoService>().As<IPhotoService>();
            container.RegisterType<FixItTaskRepository>().As<IFixItTaskRepository>();

            container.Build();

            DbConfiguration.SetConfiguration(new EFConfiguration());
        }
    }
}
