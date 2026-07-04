using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.LMS.Shared.Auth;
using PRN232.LMS.Shared.Configuration;
using PRN232.LMS.Shared.Extensions;
using PRN232.LMS.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddLmsSerilog("api-gateway");
builder.Services.AddLmsJwtAuth(builder.Configuration);
builder.Services.AddLmsSwagger("PRN232 LMS API Gateway", "v1");
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("api-gateway"));
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation();
        tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
        });
    });

var app = builder.Build();

app.UseLmsRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
