using Hangfire;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Bekam.Application.Abstraction.Contracts;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;
using Bekam.Application.Abstraction.Contracts.Security;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Contracts.Services.Identity;
using Bekam.Application.Abstraction.Contracts.Services.Payments;
using Bekam.Infrastructure.Persistence._Data;
using Bekam.Infrastructure.Persistence._Data.Interceptors;
using Bekam.Infrastructure.Persistence._Identity;
using Bekam.Infrastructure.Persistence.UnitOfWork;
using Bekam.Infrastructure.Redis;
using Bekam.Infrastructure.Security;
using Bekam.Infrastructure.Services;
using Bekam.Infrastructure.Services.Identity;


namespace Bekam.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Application Context

        services.AddDbContext<AppDbContext>((sp, optionsBuilder) =>
        {
            optionsBuilder
            .UseSqlServer(configuration.GetConnectionString("AppConnection"))
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
        });

        services.AddScoped<IAppDbInitializer, AppDbInitializer>();

        services.AddScoped<AuditInterceptor>();

        #endregion

        #region Identity Context

        services.AddDbContext<IdentityDbContext>((sp, optionsBuilder) =>
        {
            optionsBuilder
            .UseSqlServer(configuration.GetConnectionString("IdentityConnection"))
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()); ;
        });

        services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();

        #endregion

        services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IAuthService, AuthService>()
                .AddSingleton<IJwtProvider, JwtProvider>()
                .AddMapster()
                .AddEmailService(configuration)
                .AddHangfire(configuration)
                .AddRedis(configuration)
                .AddScoped<ICartRepository, CartRepository>()
                .AddPaymentService(configuration)
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IImageService, ImageService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IRolesService, RolesService>()
                .AddScoped<IUsersService, UsersService>();

        return services;
    }


    public static IServiceCollection AddPaymentService(this IServiceCollection services, IConfiguration _config)
    {
        services.Configure<StripeSettings>(options =>
        {
            options.SecretKey = _config["StripeSettings:SecretKey"]!;
            options.PublishableKey = _config["StripeSettings:PublishableKey"]!;
            options.WhSecret = _config["StripeSettings:WhSecret"]!;
        });

        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(AssemblyInformation).Assembly);

        services.AddSingleton<IMapper>(new Mapper(config));

        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MailSettings>()
                .BindConfiguration(nameof(MailSettings))
                .ValidateDataAnnotations();

        services.AddSingleton<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBackgroundJobService, HangfireJobService>();

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionString = configuration.GetConnectionString("Redis");

            var options = ConfigurationOptions.Parse(connectionString!);
            options.AbortOnConnectFail = false;

            return ConnectionMultiplexer.Connect(options);
        });

        return services;
    }
}
