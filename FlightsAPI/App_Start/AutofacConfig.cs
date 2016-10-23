using Autofac;
using Autofac.Integration.WebApi;
using FlightsApi.BusinessLayer;
using FlightsRepository;
using System.Reflection;
using System.Web.Http;

namespace FlightsAPI
{
    public static class AutofacConfig
    {
        private static readonly Assembly ThisAssembly = Assembly.GetExecutingAssembly();

        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(ThisAssembly).InstancePerRequest();
            
            builder.RegisterType<FlightRepository>().As<IFlightRepository>().InstancePerRequest();
            builder.RegisterType<ScheduleManager>().As<IScheduleManager>().InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

    }
}