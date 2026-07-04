using Microsoft.AspNetCore.HttpOverrides;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.LMS.Shared.Auth;
using PRN232.LMS.Shared.Configuration;
using PRN232.LMS.Shared.Extensions;
using PRN232.LMS.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddLmsSerilog("api-gateway");
builder.Services.AddLmsJwtAuth(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLmsSwagger("PRN232 LMS API Gateway", "v1");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
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

app.UseForwardedHeaders();
app.UseLmsRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway");
    options.SwaggerEndpoint("/swagger/identity/v1/swagger.json", "Identity Service");
    options.SwaggerEndpoint("/swagger/student/v1/swagger.json", "Student Service v1");
    options.SwaggerEndpoint("/swagger/student/v2/swagger.json", "Student Service v2");
    options.SwaggerEndpoint("/swagger/course/v1/swagger.json", "Course Service");
});
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
