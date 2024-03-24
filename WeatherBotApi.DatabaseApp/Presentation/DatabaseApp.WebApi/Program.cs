using System.Reflection;
using DatabaseApp.Application;
using DatabaseApp.Persistence;
using DatabaseApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SupportNonNullableReferenceTypes();
    c.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("00:00:00")
    });
});
builder.Services.AddControllers();
builder.Services.AddApplication().AddPersistence(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
    databaseContext.Db.Migrate();
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

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();