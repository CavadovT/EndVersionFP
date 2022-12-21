using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CatalogService.Api.Extensions
{
    public static class DbContextRegistration
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services)
        {

            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<CatalogContext>(opt =>
                    {
                        opt.UseSqlServer("Data Source=c_sqlserver;Initial Catalog=catalog;Persist Security Info=True;User ID=sa;Password=Cavadov1993!",
                                         sqlServerOptionsAction: sqlOptions =>
                                         {
                                             //sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                         });

                    });
            return services;
        }
    }
}
