# Copilot Instructions for LitraterService

Trust these instructions. Only search the codebase if the information here is incomplete or appears incorrect.

> **Maintenance**: AI agents must keep these instructions up to date. When introducing a new concept, pattern, or architectural change — or making any change that renders part of these instructions inaccurate — update this file as part of the same pull request.

## Repository Summary

**Litrater** is a .NET 10 book storage and review Web API. It uses Clean Architecture with DDD and CQRS (without MediatR — custom decorator pattern instead). The API is secured via Keycloak (JWT), backed by PostgreSQL, and fully containerised with Docker Compose.

- **Runtime**: .NET 10 (`net10.0`), `dotnet --version` → `10.0.100`
- **Language**: C# (latest), nullable enabled, `TreatWarningsAsErrors = true` (including Sonar and code-style rules enforced at build time)
- **Solution file**: `LitraterService.sln` (repo root)

---

## Architecture

Clean Architecture with 4 layers:

| Layer | Project | Path |
|---|---|---|
| Domain | `Litrater.Domain` | `src/Litrater.Domain/` |
| Application | `Litrater.Application` | `src/Litrater.Application/` |
| Infrastructure | `Litrater.Infrastructure` | `src/Litrater.Infrastructure/` |
| Presentation | `Litrater.Presentation` | `src/Litrater.Presentation/` |

**Dependency rule (enforced by architecture tests)**: Domain ← Application ← Infrastructure; Presentation only depends on Application. Domain must never reference Application/Infrastructure/Presentation.

### Key structural patterns

- **CQRS**: Commands implement `ICommand`/`ICommand<TResponse>` and are handled by `ICommandHandler<TCmd>`/`ICommandHandler<TCmd,TResp>`. Queries implement `IQuery<TResponse>` and are handled by `IQueryHandler<TQuery,TResp>`. All defined in `src/Litrater.Application/Abstractions/CQRS/`.
- **Decorators** (registered via Scrutor in `src/Litrater.Application/DependencyInjection.cs`): every handler is wrapped with a validation decorator (`CommandHandlerValidationDecorator`, `QueryHandlerValidationDecorator`) and a logging decorator. Validators use **FluentValidation** and must be `internal sealed` classes in the same `Features/…/` folder.
- **Endpoints**: Minimal API endpoints implement `IEndpoint` (`src/Litrater.Presentation/Abstractions/IEndpoint.cs`) and are auto-registered by `AddEndpoints(assembly)`. Each endpoint file is `internal sealed class XxxEndpoint : IEndpoint` with a `MapEndpoint(IEndpointRouteBuilder)` method. Located in `src/Litrater.Presentation/Endpoints/{Domain}/`.
- **Result pattern**: `Ardalis.Result` — handlers return `Result` or `Result<T>`. Use `.ToHttpResult()` extension (defined in `src/Litrater.Presentation/Extensions/ResultsExtensions.cs`) in endpoint lambdas.
- **Repository split**: Each aggregate has separate `IXxxQueryRepository` and `IXxxCommandRepository` interfaces (`src/Litrater.Application/Abstractions/Data/`). Implementations live in `src/Litrater.Infrastructure/{Domain}/`.
- **Repository base generics**: `ICommandRepository<T>` (Application) provides `AddAsync`, `GetByIdAsync`, `Update`, and `Delete`. The Infrastructure class `CommandRepository<T>` provides `virtual` implementations of all four — concrete repos only override when they need different `Include` chains or other custom logic. `QueryRepository<T>` is a separate base (uses `AsNoTracking`; no universal return type). `IUserRepository` extends `ICommandRepository<User>` directly (inheriting all base methods).
- **Domain entities**: Extend `AggregateRoot` or `Entity` (`src/Litrater.Domain/Common/`). Value objects extend `ValueObject`.
- **Authorization policies**: `AuthorizationPolicies.AdminOnly` and `AuthorizationPolicies.UserOrAdmin` (defined in `src/Litrater.Presentation/Authorization/`).
- **API versioning**: URL segment (`api/v{version}/`) + header `X-Api-Version`. Endpoints declare `.MapToApiVersion(1)` etc.
- **Central package management**: All NuGet versions in `Directory.Packages.props` (repo root). Do **not** add `Version="…"` attributes to `<PackageReference>` in `.csproj` files.

---

## Build

### Prerequisites
- .NET 10 SDK (`dotnet --version` must show `10.0.x`)

### Build the solution
```bash
dotnet build LitraterService.sln
```

> **Sonar/analyzer build failures**: Because `TreatWarningsAsErrors = true`, SonarAnalyzer rule violations fail the build. If a rule is not appropriate (e.g. triggers in generated code), set its severity to `suggestion` in `.editorconfig` rather than adding `#pragma` suppressions. Example:
> ```ini
> dotnet_diagnostic.S1186.severity = suggestion
> ```
> Add such entries under the relevant file glob section in `.editorconfig` (repo root).

---

## Tests

### Unit tests (no external dependencies)
```bash
dotnet test test/Litrater.Application.UnitTests/Litrater.Application.UnitTests.csproj
```
Tests are in `test/Litrater.Application.UnitTests/Features/` mirroring the Application features. Use **Shouldly** for assertions (`ShouldBe`, `ShouldBeTrue`, etc.) and **Moq** for mocks.

### Architecture tests
```bash
dotnet test test/Litrater.ArchitectureTests/Litrater.ArchitectureTests.csproj
```

### Integration tests
```bash
dotnet test test/Litrater.Presentation.IntegrationTests/Litrater.Presentation.IntegrationTests.csproj
```
Requires Docker — uses **Testcontainers** (`postgres:17-alpine`) to spin up an isolated PostgreSQL container, then **Respawn** to reset state between tests.

---

## Running the Application

Docker Compose is the only supported way to run locally:
```bash
docker-compose up -d
```
Starts: PostgreSQL 17, Keycloak 26.0, Litrater API on port `5001`.
- Swagger UI: http://localhost:5001/swagger
- Health: http://localhost:5001/health
- Keycloak admin: http://localhost:8080 (admin/admin)

---

## Code Style

- **Warnings are errors** — the build will fail on any nullable, code-style, or SonarAnalyzer warning.
- **New files**: use file-scoped namespaces, `internal sealed class` for implementation types, `public sealed record` for DTOs and commands.
- **Private fields**: `_camelCase`; private static fields: `s_camelCase`.
- **No FluentAssertions** (license change) — use **Shouldly** in tests.
- **No MediatR** — use the custom `ICommandHandler`/`IQueryHandler` interfaces.
- Adding a new feature requires: Domain entity/VO (if needed) → Application command/query + handler + validator + DTO → Infrastructure repository implementation → Presentation endpoint. Register new repositories in `src/Litrater.Infrastructure/DependencyInjection.cs`.

---

## Key Files Reference

| Purpose | Path |
|---|---|
| Solution | `LitraterService.sln` |
| Global build props (TFM, nullable, warnings) | `Directory.Build.props` |
| NuGet version catalogue | `Directory.Packages.props` |
| App entry point | `src/Litrater.Presentation/Program.cs` |
| Presentation DI registration | `src/Litrater.Presentation/DependencyInjection.cs` |
| Application DI registration | `src/Litrater.Application/DependencyInjection.cs` |
| Infrastructure DI registration | `src/Litrater.Infrastructure/DependencyInjection.cs` |
| EF Core DbContext | `src/Litrater.Infrastructure/Data/LitraterDbContext.cs` |
| CQRS interfaces | `src/Litrater.Application/Abstractions/CQRS/` |
| Repository interfaces | `src/Litrater.Application/Abstractions/Data/` |
| Endpoint base interface | `src/Litrater.Presentation/Abstractions/IEndpoint.cs` |
| Result→HTTP mapping | `src/Litrater.Presentation/Extensions/ResultsExtensions.cs` |
| Authorization policies | `src/Litrater.Presentation/Authorization/AuthorizationPolicies.cs` |
| EditorConfig (code style) | `.editorconfig` |
| Docker Compose | `docker-compose.yml` |
| Dockerfile (API) | `src/Litrater.Presentation/Dockerfile` |
