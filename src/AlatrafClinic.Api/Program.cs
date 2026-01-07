using AlatrafClinic.Api;
using AlatrafClinic.Application;
using AlatrafClinic.Infrastructure;
using AlatrafClinic.Infrastructure.Data;

using Scalar.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration));

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

app.UseMiddleware<IdempotencyMiddleware>();
//app.UseCoreMiddlewares(builder.Configuration, app.Environment);

app.MapControllers();

// app.UseAntiforgery();

app.MapStaticAssets();


app.Run();