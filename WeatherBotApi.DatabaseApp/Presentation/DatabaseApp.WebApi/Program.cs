using System.Reflection;
using DatabaseApp.Application;
using DatabaseApp.IntegrationEvents;
using DatabaseApp.Persistence;
using DatabaseApp.Persistence.DatabaseContext;
using DatabaseApp.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Serilog;
using TelegramBotApp.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SupportNonNullableReferenceTypes();
    c.MapType<TimeSpan>(() => new()
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddApplication().AddPersistence(builder.Configuration).AddMessaging(builder.Configuration);

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();
    var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
    databaseContext.Db.Migrate();
}
catch (Exception e)
{
    Log.Fatal(e, "An error occurred while migrating the database.");
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatabaseApp.WebApi v1");
        c.RoutePrefix = string.Empty;
    });
}

app.SubscribeToEvents();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseRouting();
app.MapControllers();

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program;