FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.sln .
COPY WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Domain/*.csproj  ./WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Domain/
COPY WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Application/*.csproj  ./WeatherBotApi.TelegramBotApp/Core/TelegramBotApp.Application/
COPY WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Messaging/*.csproj  ./WeatherBotApi.TelegramBotApp/Infrastructure/TelegramBotApp.Messaging/
COPY WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/*.csproj  ./WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Application/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Application/
COPY WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/*.csproj ./WeatherBotApi.WeatherApp/Core/WeatherApp.Domain/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/*.csproj WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.Converters/
COPY WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.IntegrationEvents/*.csproj ./WeatherBotApi.WeatherApp/Infrastructure/WeatherApp.IntegrationEvents/
COPY WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/*.csproj ./WeatherBotApi.WeatherApp/Presentation/WeatherApp.WebApi/
COPY WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/*.csproj ./WeatherBotApi.WeatherApp/Tests/WeatherApp.Tests/

RUN dotnet restore

COPY WeatherBotApi.TelegramBotApp/ ./WeatherBotApi.TelegramBotApp/
COPY WeatherBotApi.WeatherApp/ ./WeatherBotApi.WeatherApp/

WORKDIR /app/WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/WeatherBotApi.TelegramBotApp/Presentation/TelegramBotApp.Api/out .
ENTRYPOINT ["dotnet", "TelegramBotApp.Api.dll"]