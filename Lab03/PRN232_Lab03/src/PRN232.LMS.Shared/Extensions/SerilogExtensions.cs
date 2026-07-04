using Microsoft.AspNetCore.Builder;
using Serilog;

namespace PRN232.LMS.Shared.Extensions
{
    public static class SerilogExtensions
    {
        public static WebApplicationBuilder AddLmsSerilog(this WebApplicationBuilder builder, string serviceName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", serviceName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File($"logs/{serviceName}-.log", rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.Host.UseSerilog();
            return builder;
        }

        public static WebApplication UseLmsRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            });
            return app;
        }
    }
}
