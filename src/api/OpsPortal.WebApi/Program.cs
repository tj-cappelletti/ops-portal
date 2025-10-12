namespace OpsPortal.WebApi;

public class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;

                config
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables("OPSPORTAL_")
                    .AddCommandLine(args);

                if (env.IsDevelopment()) config.AddUserSecrets<Startup>();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options =>
                {
                    // Configure Kestrel options
                    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
                    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
                });
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();

                if (!hostingContext.HostingEnvironment.IsDevelopment())
                {
                    // Add production logging providers
                    // logging.AddApplicationInsights();
                }
            });
    }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}
