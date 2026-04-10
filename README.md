# Business Scenario API

A layered ASP.NET Core Web API for managing business customers and generating account statements. The backend follows a Clean Onion Architecture with Domain, Application, and Infrastructure layers, adhering to SOLID principles and utilizing the CQRS pattern via MediatR.

## Repository layout
- `BusinessScenarioAPI/` — ASP.NET Core Web API project (controllers, DI, Swagger).
- `Application/` — CQRS handlers (MediatR commands/queries), DTOs, AutoMapper profiles, FluentValidation validators.
- `Domain/` — Domain entities (`Customer`, `AccountStatement`) and repository interfaces.
- `Infrastructure/` — Persistence, EF Core DbContext, repository implementations, and external services (e.g., `EmailService`).
- `Tests/` — Unit tests using xUnit and Moq.

## Key implementation notes
- **Clean Architecture**: The solution separates concerns into discrete layers. The API layer focuses solely on taking HTTP requests and returning responses.
- **CQRS Pattern**: Business logic is encapsulated in MediatR handlers inside the `Application` layer (e.g., `CreateCustomerHandler`, `GenerateStatementsHandler`). Controllers orchestrate requests to these handlers.
- **Data Access**: Built using Entity Framework Core for SQL Server. The Repository pattern is utilized through interfaces defined in `Domain/Interfaces` and implemented in `Infrastructure/Persistence/Repositories`.
- **Validation & Mapping**: FluentValidation is used to ensure commands/queries are well-formed before processing. AutoMapper translates between Domain entities and DTOs.
- **Testing**: Application handlers are thoroughly unit-tested using xUnit and Moq.

## Tech stack
- .NET 8 / C#
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- MediatR for CQRS
- AutoMapper
- FluentValidation
- xUnit & Moq for unit testing
- Swagger/OpenAPI for API documentation

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express/LocalDB)
- IDE: Visual Studio 2022, Rider, or VS Code

## Quick setup

1. Clone the repository and navigate to the directory:
   ```bash
   git clone https://github.com/Anaszor/AccountBusinessScenario.git
   cd AccountBusinessScenario
   ```

2. Restore NuGet dependencies:
   ```bash
   dotnet restore
   ```

3. **Application Configuration**:
   Update the `BusinessScenarioAPI/appsettings.json` (or `appsettings.Development.json`) to include your database and SMTP configurations. Make sure to update the placeholders with your actual server and email credentials:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=<your_server_name>;Database=BusinessScenarioDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
     },
     "Smtp": {
       "Host": "smtp.gmail.com",
       "Port": "587",
       "User": "<your_email@gmail.com>",
       "Pass": "<your_app_password>"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

4. **Apply database migrations**:
   Ensure you have the EF Core tools installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
   Then apply migrations:
   ```bash
   dotnet ef database update -p Infrastructure/Infrastructure.csproj -s BusinessScenarioAPI/BusinessScenarioAPI.csproj
   ```

5. **Run the API**:
   ```bash
   dotnet run --project BusinessScenarioAPI/BusinessScenarioAPI.csproj
   ```

   *If using Visual Studio: Open the `.sln` file, set `BusinessScenarioAPI` as the Startup Project, and press F5 to run.*

## Running tests
To execute all unit tests in the `Tests` project, run:
```bash
dotnet test
```

## Development & contribution
- Follow **Clean Architecture** conventions. Do not add logic dependencies outside their appropriate layers.
- Keep **Controllers thin**. Leverage MediatR to push operations into the Application handlers.
- Always add unit tests for new application handlers or complex domain logic. Mock dependencies using Moq.
- Ensure mapping profiles and validation rules are continuously updated to reflect domain changes.
