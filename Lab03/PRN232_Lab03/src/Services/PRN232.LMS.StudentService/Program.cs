using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.LMS.Shared.Auth;
using PRN232.LMS.Shared.Configuration;
using PRN232.LMS.Shared.Extensions;
using PRN232.LMS.Shared.Middleware;
using PRN232.LMS.StudentService.Data;
using PRN232.LMS.StudentService.Grpc;
using PRN232.LMS.StudentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

builder.AddLmsSerilog("student-service");
builder.Services.AddControllers();
builder.Services.AddLmsJwtAuth(builder.Configuration);
builder.Services.AddLmsSwagger("PRN232 LMS Student Service", "v1");
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddMvc().AddApiExplorer();

builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("StudentDb"),
        b => b.MigrationsAssembly("PRN232.LMS.StudentService")
    )
);

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("student-service"));
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
    var dbContext = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
    await StudentDbSeeder.SeedAsync(dbContext);
}

app.UseLmsRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<StudentGrpcService>();
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
