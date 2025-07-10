using Serilog;
using Template.Api.Configuration;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Kestrel
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.ListenAnyIP(5271); // HTTP
        serverOptions.ListenAnyIP(7185, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS
        });
    });

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddApplicationServices();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Configure HTTPS
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 7185;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Only use HTTPS redirection in production
    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    // Add authentication middleware
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Log startup
    Log.Information("Starting up {ApplicationName} in {Environment} environment on ports: HTTP:5271, HTTPS:7185, swagger UI: /swagger/index.html",
        builder.Environment.ApplicationName,
        builder.Environment.EnvironmentName);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}