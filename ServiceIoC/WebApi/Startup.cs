#define UNITY 

using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Core.Infrastructure;
using Microsoft.Practices.Unity;
using Owin;
using Unity.WebApi;

namespace WebApi
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.MapHttpAttributeRoutes();

            ConfigureAutofac(appBuilder,config);
            ConfigureUnity(appBuilder, config);

            appBuilder.Use(typeof(MyOwinComponent));
            appBuilder.UseWebApi(config);
        }

        [Conditional("UNITY")]
        private static void ConfigureUnity(IAppBuilder appBuilder, HttpConfiguration config)
        {
            var container = new UnityContainer();

            container.RegisterType<IActorFactory, ReliableFactory>();
            container.RegisterType<IServiceFactory, ReliableFactory>();

            config.DependencyResolver = new UnityDependencyResolver(container);
        }

        [Conditional("AUTOFAC")]
        private static void  ConfigureAutofac(IAppBuilder appBuilder, HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.Register<IActorFactory>(a => new ReliableFactory());
            builder.Register<IServiceFactory>(a => new ReliableFactory());

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);


            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
        }
    }
}
