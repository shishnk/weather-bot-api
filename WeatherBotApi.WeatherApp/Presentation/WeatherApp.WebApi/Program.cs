using System.Text.Json;
using Converters.JsonConverters;
using TelegramBotApp.Messaging;
using WeatherApp.IntegrationEvents;
using WeatherApp.WeatherStation.Services;

var builder = WebApplication.CreateBuilder(args);
var jsonSerializerOptions = new JsonSerializerOptions
{
    Converters = { new WeatherDescriptorJsonConverter() }
};

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddControllers();
builder.Services.AddSingleton(jsonSerializerOptions);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApp.WebApi v1");
        c.RoutePrefix = string.Empty;
    });
}

app.SubscribeToEvents();
app.UseHttpsRedirection();
app.MapHealthChecks("/health");
app.UseRouting();
app.MapControllers();

app.Run();