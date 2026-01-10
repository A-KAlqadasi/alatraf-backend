using System.Diagnostics;

using AlatrafClinic.Api;
using AlatrafClinic.Application;
using AlatrafClinic.Application.Sagas;
using AlatrafClinic.Application.Sagas.Compensation;
using AlatrafClinic.Infrastructure;
using AlatrafClinic.Infrastructure.BackgroundServices;
using AlatrafClinic.Infrastructure.Data;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ISagaStateService, SagaStateService>();
builder.Services.AddScoped<ISagaCompensationHandler, SaleSagaCompensationHandler>();
builder.Services.AddScoped<ISagaCompensationCoordinator, SagaCompensationCoordinator>();
builder.Services.AddHostedService<FailedSagaProcessor>();


builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));


// In your Startup/Program.cs
builder.Services.AddHttpClient("Default")
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    })
    .SetHandlerLifetime(TimeSpan.FromSeconds(5));


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
        "SagaId={SagaId} SaleId={SaleId}{NewLine}{Exception}"
    )
    .CreateLogger();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AlatrafClinic API V1");

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
    await app.InitialiseDatabaseAsync();


    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}
app.UseRouting();
app.UseMiddleware<IdempotencyMiddleware>();


app.UseCoreMiddlewares(builder.Configuration, app.Environment);
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request: {context.Request.Method} {context.Request.Path}");

    var stopwatch = Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    logger.LogInformation($"Response: {context.Response.StatusCode} - {stopwatch.ElapsedMilliseconds}ms");
});

app.MapControllers();

// app.UseAntiforgery();

app.MapStaticAssets();


app.Run();


app.MapGet("/di-test", (IServiceProvider sp) =>
{
    var svc = sp.GetService<ISagaStateService>();
    return svc is null ? "NULL" : "RESOLVED";
});
