using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.RateLimiting;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.API.Filters;
using Bekam.API.Services;
using Bekam.Application;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Security;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Domain.Entities.Identity;
using Bekam.Infrastructure;
using Bekam.Infrastructure.Persistence._Identity;
namespace Bekam.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddApplication()
                .AddInfrastructure(configuration)
                .AddHttpContextAccessor()
                .AddScoped<ILoggedInUserService, LoggedInUserService>()
                .AddApiServices()
                .AddIdentityConfigs()
                .AddAuthConfigs(configuration)
                .AddCorsConfigs(configuration)
                .AddScoped<IUrlBuilder, UrlBuilder>()
                .AddRateLimitingConfig();



        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true; 
        }); ;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddIdentityConfigs(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(
            identityOptions =>
            {
                identityOptions.User.RequireUniqueEmail = true;

                identityOptions.SignIn.RequireConfirmedEmail = true;

                identityOptions.Password.RequireNonAlphanumeric = true;
                identityOptions.Password.RequiredLength = 8;
                identityOptions.Password.RequireDigit = true;
                identityOptions.Password.RequireLowercase = true;
                identityOptions.Password.RequireUppercase = true;


                identityOptions.Lockout.AllowedForNewUsers = true; // default value when create user
                identityOptions.Lockout.MaxFailedAccessAttempts = 5;
                identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddAuthConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings?.Issuer,
                ValidAudience = jwtSettings?.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings?.Key!)),

                NameClaimType = JwtRegisteredClaimNames.Name                
            };
        });

        return services;
    }

    private static IServiceCollection AddCorsConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular",
                policy =>
                {
                    policy
                        .WithOrigins(configuration["AppSettings:FrontEndUrl"]!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }

    private static IServiceCollection AddRateLimitingConfig(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            rateLimiterOptions.AddPolicy(policyName: RateLimiters.IpLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1)
                    }
                )
            );

            rateLimiterOptions.AddPolicy(policyName: RateLimiters.StrictIpLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1)
                    }
                )
            );

            rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.GetUserId() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromSeconds(60)
                    }
                )
            );

            rateLimiterOptions.AddPolicy(RateLimiters.MixedLimiter, context =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User.GetUserId(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1)
                        })!;
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 60,
                        Window = TimeSpan.FromMinutes(1)
                    });
            });

            rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
            {
                options.PermitLimit = 50;
                options.QueueLimit = 20;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }





}