using Autofac;
using System.Reflection;

namespace BasketService.Api.AutoFac
{
    public class AutofacModule: Autofac.Module
    {
          
        protected override void Load(ContainerBuilder builder)
        {
            // scan all assemblies in current application domain and resolve them on convention
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();
        }
    }
}
