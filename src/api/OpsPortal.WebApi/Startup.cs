using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpsPortal.Infrastructure.Persistence;
using OpsPortal.WebApi.Extensions;

namespace OpsPortal.WebApi;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();

            // Ensure database is created in development67
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OpsPortalDbContext>();
            dbContext.Database.EnsureCreated();
        }
        else
        {
            // Production error handling
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("OpsPortalPolicy");

        // Add authentication middleware here when ready
        // app.UseAuthentication();
        // app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });
        });
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpsPortalSwagger();

        // Use extension methods for complex configurations
        services.AddOpsPortalDatabase(_configuration, _environment);
        services.AddOpsPortalMediatR();
        services.AddOpsPortalServices();
        services.AddOpsPortalHealthChecks(_configuration);
        services.AddOpsPortalCors(_configuration, _environment);
    }
}
