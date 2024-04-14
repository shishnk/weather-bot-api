FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Application/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Application/
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/*.csproj WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.IntegrationEvents/*.csproj ./WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.IntegrationEvents/
COPY WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/*.csproj ./WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/ 
COPY WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/*.csproj ./WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/
COPY WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Domain/*.csproj ./WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Domain/
COPY WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Application/*.csproj ./WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Application/
COPY WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Messaging/*.csproj ./WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Messaging/
COPY WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Caching/*.csproj ./WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Caching/
COPY WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/*.csproj ./WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/
COPY WeatherBotApi.DatabaseApp/Core/DatabaseApp.Domain/*.csproj ./WeatherBotApi.DatabaseApp/Core/DatabaseApp.Domain/
COPY WeatherBotApi.DatabaseApp/Core/DatabaseApp.Application/*.csproj ./WeatherBotApi.DatabaseApp/Core/DatabaseApp.Application/
COPY WeatherBotApi.DatabaseApp/Infrastructure/DatabaseApp.Persistence/*.csproj ./WeatherBotApi.DatabaseApp/Infrastructure/DatabaseApp.Persistence/
COPY WeatherBotApi.DatabaseApp/Infrastructure/DatabaseApp.IntegrationEvents/*.csproj ./WeatherBotApi.DatabaseApp/Infrastructure/DatabaseApp.IntegrationEvents/
COPY WeatherBotApi.DatabaseApp/Presentation/DatabaseApp.WebApi/*.csproj ./WeatherBotApi.DatabaseApp/Presentation/DatabaseApp.WebApi/
COPY WeatherBotApi.DatabaseApp/Tests/DatabaseApp.Tests/*.csproj ./WeatherBotApi.DatabaseApp/Tests/DatabaseApp.Tests/

RUN dotnet restore

COPY WeatherBotApi.TelegramBotApp/ ./WeatherBotApi.TelegramBotApp/
COPY WeatherBotApi.WeatherApp/ ./WeatherBotApi.WeatherApp/
COPY WeatherBotApi.DatabaseApp/ ./WeatherBotApi.DatabaseApp/

WORKDIR /app/WeatherBotApi.DatabaseApp/Presentation/DatabaseApp.WebApi/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl
COPY --from=build /app/WeatherBotApi.DatabaseApp/Presentation/DatabaseApp.WebApi/out ./

ENTRYPOINT ["dotnet", "DatabaseApp.WebApi.dll"]