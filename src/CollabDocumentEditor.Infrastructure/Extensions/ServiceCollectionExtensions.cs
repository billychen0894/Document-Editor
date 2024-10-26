using System.IdentityModel.Tokens.Jwt;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using JwtBearerEvents = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents;

namespace CollabDocumentEditor.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

        if (jwtSettings == null || !jwtSettings.IsValid())
        {
            throw new InvalidOperationException("JWT settings are missing or invalid.");
        }
        
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Configure JWT Bearer auth
                options.TokenValidationParameters = JwtBearerOptionsSetup.CreateTokenValidationParameters(jwtSettings);

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenService = context.HttpContext.RequestServices.GetService<ITokenService>();

                        if (context.SecurityToken is JwtSecurityToken token && tokenService != null)
                        {
                            var validationResult = await tokenService.ValidateAccessTokenAsync(token.RawData);
                            if (!validationResult.IsValid)
                            {
                                context.Fail("Invalid token");
                            }
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            }); 
        
        return services;
    }
}