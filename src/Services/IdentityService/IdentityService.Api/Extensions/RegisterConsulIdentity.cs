using Consul;
using System.Reflection;

namespace IdentityService.Api.Extensions
{
    public static class RegisterConsulIdentity
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consuleconfig =>
            {

                var address = configuration["ConsulConfig:Address"];
                consuleconfig.Address = new Uri(address);
            }));
            return services;
        }

        public static WebApplication RegisterWithConsul(this WebApplication app, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            var consulClient = app.Services.GetRequiredService<IConsulClient>();

            var loggingFactory = app.Services.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var uri = configuration.GetValue<Uri>("ConsulConfig:ServiceAddress");
            var serviceName = configuration.GetValue<string>("ConsulConfig:ServiceName");
            var serviceId = configuration.GetValue<string>("ConsulConfig:ServiceId");

            var registration = new AgentServiceRegistration()
            {

                ID = serviceId ?? $"IdentityService",
                Name = serviceName ?? "IdentityService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { serviceName, serviceId, "Token", "JWT" }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();

            });
            return app;
        }
    }
}
