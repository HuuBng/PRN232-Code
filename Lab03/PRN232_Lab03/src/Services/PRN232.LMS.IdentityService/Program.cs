using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.LMS.IdentityService.Data;
using PRN232.LMS.IdentityService.Services;
using PRN232.LMS.Shared.Auth;
using PRN232.LMS.Shared.Configuration;
using PRN232.LMS.Shared.Extensions;
using PRN232.LMS.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddLmsSerilog("identity-service");
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
})
    .AddXmlSerializerFormatters();
builder.Services.AddLmsSwagger("PRN232 LMS Identity Service", "v1");
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddLmsJwtAuth(builder.Configuration);
builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("IdentityDb"),
        b => b.MigrationsAssembly("PRN232.LMS.IdentityService")
    )
);
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("identity-service"));
        tracing.AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation();
        tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"] ?? "http://localhost:4317");
        });
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await IdentityDbSeeder.SeedAsync(dbContext);
}

app.UseForwardedHeaders();
app.UseLmsRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
