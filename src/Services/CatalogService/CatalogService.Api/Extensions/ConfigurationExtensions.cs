namespace CatalogService.Api.Extensions
{
    public  static class ConfigurationExtensions
    {
        public static IConfiguration AddSystemConfiguration(this IConfiguration configuration,IWebHostEnvironment env) 
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json",optional:false)
                .AddJsonFile($"appsettings.{env}.json",optional:true)
                .AddEnvironmentVariables()
                .Build();
            
        }
        public static IConfiguration AddSerilogConfiguration(this IConfiguration configuration, IWebHostEnvironment env) 
        {
            return new ConfigurationBuilder()
                   .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                   .AddJsonFile($"serilog.json", optional: false)
                   .AddJsonFile($"serilog.{env}.json", optional: true)
                   .AddEnvironmentVariables()
                   .Build();
        }
    }
}
