[![License](https://img.shields.io/badge/license-MIT-green?style=for-the-badge)](./LICENSE)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/shishnk/weather-bot-api/dotnet.yml?style=for-the-badge&label=BUILD%2FTESTS)
![Coveralls](https://img.shields.io/coverallsCoverage/github/shishnk/weather-bot-api?style=for-the-badge)
![Website](https://img.shields.io/website?url=https%3A%2F%2Fwttr.in%2F&style=for-the-badge&label=wttr.in)

# WeatherBotApi

## Overview

WeatherBotApi is an educational project aimed at learning microservices development on the .NET platform and improving skills in ASP.NET, RabbitMQ, Entity Framework, PostgreSQL, Mapster, Redis, Docker, and xUnit.

## Description

WeatherBot is a Telegram bot designed to provide weather data to users. The project is implemented using a microservices architecture, consisting of three services:

1. **TelegramApi:** A service responsible for handling user requests via the Telegram bot.
2. **DatabaseService:** A service responsible for database operations, including storing and retrieving user data and their requests.
3. **WeatherService:** A service responsible for fetching weather information from an external source.

## Weather API
The [wttr.in](https://github.com/chubin/wttr.in) is used to provide weather, which is  a console-oriented weather forecast service that supports various information representation methods like terminal-oriented ANSI-sequences for console HTTP clients (curl, httpie, or wget), HTML for web browsers, or PNG for graphical viewers.


## Architecture

The following diagram illustrates the interaction between different services in the project:

```mermaid
flowchart TD;
    A[Telegram API] -->|Sends requests| B((RabbitMQ));
    B -->|Sends requests| D[Database Service];
    D -->|Retrieves data| B;
    B -->|Forwards requests| C[Weather Service];
    C -->|Retrieves weather data| B;

```

## Technology Stack

- ASP.NET
- RabbitMQ (MassTransit can be used, but it provides a high-level API)
- Entity Framework
- PostgreSQL
- Mapster
- Docker
- Redis
- xUnit

## Installation and Running

1. **Clone the repository:**
   ```bash
   git clone https://github.com/shishnk/weather-bot-api.git
   ```
2. **Build Docker containers:**
    ```bash
    git cd weather-bot-api
    docker-compose build
    ```
3. **Run containers:**
    ```bash
    docker-compose up
    ```
    
## Configuration

Before running the project, ensure you have configured the necessary environment variables:

1. **Telegram Bot Configuration:**
    - Create a new bot via [BotFather](https://t.me/botfather) on Telegram.
    - Obtain the token for accessing the bot's API.

2. **Update the existing .env file:**
    - Open file in the root directory of your project.
    - Add or update the following line in the .env file with your Telegram bot token:
      ```
      TELEGRAM_BOT_TOKEN=YOUR_TELEGRAM_BOT_TOKEN
      ```
    - Replace `YOUR_TELEGRAM_BOT_TOKEN` with your actual Telegram bot token obtained from BotFather.

Once you have updated the existing .env file with your Telegram bot token, you are ready to run the project.


## Contribution and Feedback

If you have suggestions for improving the project or found a bug, please open an issue or submit a pull request. We welcome your contributions!


## License

The project is licensed under the MIT license. For more information, see the [LICENSE](https://github.com/shishnk/weather-bot-api/blob/master/LICENSE).
