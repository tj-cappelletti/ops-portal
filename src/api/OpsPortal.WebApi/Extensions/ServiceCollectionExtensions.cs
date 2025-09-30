using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using OpsPortal.Application;
using OpsPortal.Application.Common.Interfaces;
using OpsPortal.Infrastructure.Persistence;

namespace OpsPortal.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpsPortalCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("OpsPortalPolicy", builder =>
            {
                if (environment.IsDevelopment())
                {
                    // Allow any origin in development
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                else
                {
                    // Configure specific origins for production
                    var allowedOrigins = configuration
                        .GetSection("Cors:AllowedOrigins")
                        .Get<string[]>() ?? Array.Empty<string>();

                    builder.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddOpsPortalDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("OpsPortalDb");
        var databaseProvider = configuration["DatabaseProvider"] ?? "PostgreSQL"; // Default to PostgreSQL

        services.AddDbContext<OpsPortalDbContext>(options =>
        {
            switch (databaseProvider.ToLowerInvariant())
            {
                case "postgresql":
                case "postgres":
                case "npgsql":
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                        {
                            npgsqlOptions.EnableRetryOnFailure(
                                3,
                                TimeSpan.FromSeconds(5),
                                null);
                            npgsqlOptions.CommandTimeout(30);
                            // Use snake_case naming convention for PostgreSQL
                            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        })
                        .UseSnakeCaseNamingConvention();
                    break;

                case "sqlserver":
                case "mssql":
                    options.UseSqlServer(connectionString, sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            3,
                            TimeSpan.FromSeconds(5),
                            null);
                        sqlServerOptions.CommandTimeout(30);
                    });
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
            }

            if (!environment.IsDevelopment()) return;

            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
        });

        services.AddSingleton<IDatabaseProvider>(new DatabaseProvider(databaseProvider));

        return services;
    }

    public static IServiceCollection AddOpsPortalHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //TODO: Add other health checks as needed (e.g., external services, disk space, etc.)
        //TODO: Consider building a custom health check for database connection validation
        //TODO: Consider adding a health check UI for better visualization
        services.AddHealthChecks()
            .AddDbContextCheck<OpsPortalDbContext>(
                "database",
                HealthStatus.Degraded,
                ["ready", "db"]);

        return services;
    }

    public static IServiceCollection AddOpsPortalMediatR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // Register from the Application assembly where handlers live
            cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

            // Or if you don't have an AssemblyReference marker class:
            // cfg.RegisterServicesFromAssemblyContaining<GetAllSolutionStacksHandler>();
        });

        return services;
    }

    public static IServiceCollection AddOpsPortalServices(this IServiceCollection services)
    {
        // Register all application services
        // services.AddScoped<ISolutionStackRepository, SolutionStackRepository>();
        // services.AddScoped<IGitHubService, GitHubService>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<OpsPortalDbContext>());

        return services;
    }

    public static IServiceCollection AddOpsPortalSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OpsPortal API",
                Version = "v1",
                Description = "Operations Portal API for managing solution stacks across hybrid infrastructure",
                Contact = new OpenApiContact
                {
                    Name = "OpsPortal Team",
                    Email = "admin@ops-portal.local",
                    Url = new Uri("https://github.com/tj-cappelletti/ops-portal")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Include XML comments if available
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);

            // Add security definition when authentication is implemented
            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });
        });

        return services;
    }
}
