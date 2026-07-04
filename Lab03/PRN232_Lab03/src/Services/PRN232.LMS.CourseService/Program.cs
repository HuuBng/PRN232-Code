using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Grpc;
using PRN232.LMS.CourseService.Services;
using PRN232.LMS.Protos;
using PRN232.LMS.Shared.Auth;
using PRN232.LMS.Shared.Configuration;
using PRN232.LMS.Shared.Extensions;
using PRN232.LMS.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddLmsSerilog("course-service");
builder.Services.AddControllers();
builder.Services.AddLmsJwtAuth(builder.Configuration);
builder.Services.AddLmsSwagger("PRN232 LMS Course Service", "v1");
builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("CourseDb"),
        b => b.MigrationsAssembly("PRN232.LMS.CourseService")
    )
);

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
});

// gRPC client with Polly resilience
builder.Services.AddGrpcClient<StudentGrpc.StudentGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Grpc:StudentServiceUrl"]!);
}).ConfigureChannel(o => o.Credentials = Grpc.Core.ChannelCredentials.Insecure)
.AddStandardResilienceHandler();

// Decorator: Cache wraps the gRPC client
builder.Services.AddScoped<StudentGrpcClient>();
builder.Services.AddScoped<IStudentGrpcClient>(sp =>
{
    var inner = sp.GetRequiredService<StudentGrpcClient>();
    var cache = sp.GetRequiredService<IDistributedCache>();
    var logger = sp.GetRequiredService<ILogger<CachedStudentGrpcClient>>();
    return new CachedStudentGrpcClient(inner, cache, logger);
});

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

// OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("course-service"));
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
    var dbContext = scope.ServiceProvider.GetRequiredService<CourseDbContext>();
    await CourseDbSeeder.SeedAsync(dbContext);
}

app.UseLmsRequestLogging();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.Run();
