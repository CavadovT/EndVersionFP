using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using ConsulRegistration;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

IConfiguration Configuration = builder.Configuration;
IWebHostEnvironment env = builder.Environment;
builder.WebHost.UseDefaultServiceProvider((context, opt) =>
{
    opt.ValidateOnBuild = false;
    opt.ValidateScopes = false;
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogService.Api", Version = "v1" });
});


builder.Services.Configure<CatalogSettings>(Configuration.GetSection("CatalogSettings"));
builder.Services.ConfigureDbContext();

builder.Services.ConfigureConsul(Configuration);
builder
    .Logging
    .Services
    .AddLogging(x => new LoggerConfiguration()
.ReadFrom.Configuration(Configuration.AddSerilogConfiguration(env))
.CreateLogger()
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogService.Api v1"));
}

app.UseHttpsRedirection();
//app.UseStaticFiles(new StaticFileOptions()
//{
//    FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(env.ContentRootPath, "Pics")),
//    RequestPath = "/pics"
//});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MigrateDbContext<CatalogContext>((context, services) =>
{

    //var env1 = services.GetService<IWebHostEnvironment>();
    var logger = services.GetService<ILogger<CatalogContextSeed>>();

    new CatalogContextSeed()
            .SeedAsync(context, env, logger)
            .Wait();

});

app.RegisterWithConsul(app.Lifetime, app.Configuration);

app.UseEndpoints(endpoints =>
endpoints.MapControllers()
);

app.Run();




