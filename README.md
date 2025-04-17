# NinjaBot

A .NET 8 solution for bot automation and web scraping.

## Project Structure

- **NinjaBot.Core**: Core bot logic for automation and web scraping
- **NinjaBot.API**: REST API for controlling the bot
- **NinjaBot.Worker**: Background service for executing tasks
- **NinjaBot.Web**: Blazor web interface for bot management
- **NinjaBot.Shared**: Shared DTOs and models
- **NinjaBot.Infrastructure**: Data access, configuration, and logging

## Getting Started

1. Ensure you have .NET 8 SDK installed
2. Clone the repository
3. Run `dotnet build` to build all projects
4. Run `dotnet run --project NinjaBot.API` to start the API
5. Run `dotnet run --project NinjaBot.Web` to start the web interface

## Project Dependencies

Each project has its specific responsibilities and dependencies:

- **NinjaBot.Core**: Contains core bot logic and automation
- **NinjaBot.API**: ASP.NET Core Web API with Swagger documentation
- **NinjaBot.Worker**: Background service for task execution
- **NinjaBot.Web**: Blazor web application for UI
- **NinjaBot.Shared**: Shared models and DTOs
- **NinjaBot.Infrastructure**: Database access, logging, and configuration

## Development

The solution follows Clean Architecture principles with clear separation of concerns:

1. Core business logic is isolated in NinjaBot.Core
2. API and Web projects depend on Core through abstractions
3. Infrastructure implements the abstractions defined in Core
4. Shared contains DTOs and models used across all projects
