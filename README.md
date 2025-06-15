# .NET 7 API Template with Hexagonal Architecture

This template provides a structured approach to building APIs using .NET 7, following the hexagonal architecture pattern (also known as ports and adapters). It includes authentication, logging, and Swagger documentation out of the box.

## Features

- 🏗️ **Hexagonal Architecture**
  - Core Domain Logic
  - Application Services
  - Infrastructure Implementation
  - API Controllers
  - Shared Infrastructure

- 🔐 **Authentication**
  - JWT Token-based authentication
  - Configurable JWT settings
  - Ready for custom user storage implementation

- 📝 **Logging with Serilog**
  - Console logging
  - File logging with daily rotation
  - Structured logging format
  - Request/Response logging

- 📚 **API Documentation**
  - Swagger UI
  - OpenAPI specification

## Project Structure

```
Template.Net.sln

Template.Net/                # Main API project
├── src/
│   ├── Api/                # API Endpoints, Configuration, Middleware
│   ├── Core/               # Domain, Application (Entities, Value Objects, Interfaces, DTOs, Services)
│   ├── Infrastructure/     # Database, Repositories, Service Implementations
│   └── Shared/             # Shared Components
│       └── Infrastructure/
├── appsettings.json
├── appsettings.Development.json
└── ...other project files

Template.Net.Tests/          # Test project
├── Template.Net.Tests.csproj
└── tests/
    ├── Api/                # API layer tests
    ├── Core/
    │   ├── Application/    # Application layer tests
    │   └── Domain/         # Domain layer tests
    └── Infrastructure/     # Infrastructure layer tests
```

## Quick Start

### Creating a New Project

1. Clone this template repository
2. Run the provided PowerShell script to create a new project:

```powershell
.\create-project-from-template.ps1 -NewProjectName "YourProjectName" -DestinationPath "C:\YourPath"
```

Parameters:
- `NewProjectName`: The name of your new project
- `DestinationPath`: Where to create the project (optional, defaults to parent directory)

Example:
```powershell
.\create-project-from-template.ps1 -NewProjectName "MyAwesomeApi" -DestinationPath "C:\Projects"
```

### Running the Project

1. Navigate to your project directory
2. Restore dependencies:
```powershell
dotnet restore
```

3. Run the application:
```powershell
dotnet run --project Template.Net
```

The API will be available at:
- HTTP: http://localhost:5271
- HTTPS: https://localhost:7185
- Swagger UI: http://localhost:5271/swagger

### Running Tests

To run all tests in the solution, use:
```powershell
dotnet test
```

Or to run tests for a specific project:
```powershell
dotnet test Template.Net.Tests/Template.Net.Tests.csproj
```

This will build the solution and execute all unit tests, showing the results in the terminal.

## Template Versioning with Tags

This template uses Git tags to manage different versions. Each tag represents a specific .NET version or major feature update.

### Available Tags

- `Net-7.0.0`: Base template for .NET 7.0.0 projects with hexagonal architecture

### Working with Tags

#### Getting a Specific Version

To create a project from a specific version:

1. Fetch all tags:
```powershell
git fetch --all --tags
```

2. Check out the desired tag:
```powershell
git checkout tags/Net-7.0.0
```

3. Then run the create-project script as normal.

#### Creating New Tags (For Contributors)

1. Make your changes and commit them
2. Create a new tag:
```powershell
git tag -a Net-X.Y.Z -m "Description of this version"
```

3. Push the tag:
```powershell
git push origin Net-X.Y.Z
```

#### Updating Existing Tags

If you need to update an existing tag:

1. Delete the tag locally and remotely:
```powershell
git tag -d Net-X.Y.Z
git push origin :refs/tags/Net-X.Y.Z
```

2. Create the tag again at the desired commit:
```powershell
git tag -a Net-X.Y.Z -m "Updated description"
git push origin Net-X.Y.Z
```

## Authentication

### Configuration

JWT settings can be configured in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "your-api",
    "Audience": "your-clients",
    "ExpiryInMinutes": 60
  }
}
```

### Getting a Token

Make a POST request to obtain a JWT token:

```http
POST http://localhost:5271/api/auth/login
Content-Type: application/json

{
    "username": "demo",
    "password": "demo123"
}
```

### Using Authentication

1. Get a token using the login endpoint
2. Add the token to your requests:
```http
GET http://localhost:5271/weatherforecast
Authorization: Bearer your-token-here
```

## Logging

Logs are written to:
- Console (for development)
- `Logs/log-YYYYMMDD.txt` (daily rotating files)

## Development Notes

1. Add new entities in `Core/Domain/Entities`
2. Define interfaces in `Core/Application/Interfaces`
3. Implement services in `Infrastructure/Services`
4. Add controllers in `Api/Controllers`

## Best Practices

1. Keep the domain layer independent of infrastructure concerns
2. Use interfaces to define contracts in the application layer
3. Implement cross-cutting concerns in the infrastructure layer
4. Use dependency injection for loose coupling
5. Follow REST API best practices in controllers

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This template is licensed under the MIT License - see the LICENSE file for details.