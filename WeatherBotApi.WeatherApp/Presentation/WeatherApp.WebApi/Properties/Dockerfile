FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Application/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Application/
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/*.csproj WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.MassTransitIntegration/*.csproj ./WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.MassTransitIntegration/
COPY WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/*.csproj ./WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/ 
COPY WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/*.csproj ./WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/
COPY WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/*.csproj ./WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/

RUN dotnet restore

COPY WeatherBotApi.WeatherApp/ ./WeatherBotApi.WeatherApp/
COPY WeatherBotApi.TelegramBotApp/ ./WeatherBotApi.TelegramBotApp/

WORKDIR /app/WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/out ./

ENTRYPOINT ["dotnet", "WeatherApp.WebApi.dll"]