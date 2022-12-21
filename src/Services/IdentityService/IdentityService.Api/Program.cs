using IdentityService.Api.Application.Services;
using IdentityService.Api.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

IConfiguration Configuration=builder.Configuration;
IWebHostEnvironment env=builder.Environment;

builder.WebHost.UseDefaultServiceProvider((context, opt) =>
{
    opt.ValidateOnBuild = false;
    opt.ValidateScopes = false;

});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => 
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityService.Api", Version = "v1" });
});

builder.Services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
builder.Services.ConfigureConsul(builder.Configuration);

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
    app.UseSwagger();
    app.UseSwaggerUI(c=>c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.Api v1"));
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();

app.RegisterWithConsul(app.Lifetime,app.Configuration);

app.MapControllers();

app.Run();
