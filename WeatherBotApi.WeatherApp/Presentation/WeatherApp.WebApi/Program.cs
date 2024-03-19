using System.Text.Json;
using Converters.JsonConverters;
using WeatherApp.Application.Services;
using WeatherApp.RabbitMqIntegration.RabbitMqConsumer;

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
builder.Services.AddHostedService<MessageConsumer>();

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

// app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();