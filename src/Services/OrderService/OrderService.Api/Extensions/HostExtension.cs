using Microsoft.EntityFrameworkCore;
using Polly;
using System.Data.SqlClient;

namespace OrderService.Api.Extensions
{
    public static class HostExtension
    {
        public static WebApplication MigrateDbContext<TContext>(this WebApplication app, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });

                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database");
                }
                catch (Exception ex)
                {
                    logger.LogInformation("An error occured while migration the database used on context {DbContextName}", typeof(TContext).Name);
                }
            }
            return app;
        }
        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            seeder(context, services);

        }
    }
}
