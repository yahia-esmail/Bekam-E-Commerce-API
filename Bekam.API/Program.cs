using Hangfire;
using Serilog;
using Bekam.API.Extensions;
using Bekam.API.Middleware;
using Bekam.Application.Abstraction.Contracts.Services;

namespace Bekam.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        #region Configure Services
        builder.Services.AddDependencies(builder.Configuration);
        #endregion
                
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        var app = builder.Build();

        #region Databases Initialization
        await app.InitializeDbAsync();
        #endregion

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboard("/jobs", new DashboardOptions()
            {
                DashboardTitle = "Bekam Jobs Dashboard"
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        app.UseCors("AllowAngular");

        app.UseSerilogRequestLogging();

        app.UseHttpsRedirection();


        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseAuthentication();

        app.UseMiddleware<StrictAuthorizationMiddleware>(); 

        app.UseAuthorization();

        app.UseStaticFiles();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();
    }
}
