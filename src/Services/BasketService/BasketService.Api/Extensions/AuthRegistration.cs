﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BasketService.Api.Extensions
{
    public static class AuthRegistration
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration config) 
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["AuthConfig:Secret"]));

            services.AddAuthentication(opt => {

                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters 
                {
                ValidateIssuer=false,
                ValidateAudience=false,
                ValidateLifetime=true,
                ValidateIssuerSigningKey=true,
                IssuerSigningKey=signingKey,
                };
            });

            return services;

        }
    }
}
