# Tech Stack Template

Tech stack template based on the NTRS 2.0 project.  
Serves as a starting point for creating new projects using the same technologies.

---

## 🏗️ Solution Architecture

The solution consists of **4 projects** in a single `.sln`:

```
Solution/
├── ProjectName/              # Frontend — Blazor Server
├── ProjectName.Api/          # Backend — ASP.NET Core REST API
├── ProjectName.Database/     # Data layer — EF Core + PostgreSQL
├── ProjectName.Common/       # Shared models/DTOs (optional)
└── ProjectName.Tests/        # Unit tests
```

Communication: Frontend → REST API (HTTP/JSON) → EF Core → PostgreSQL

---

## 📦 Projekty i paczki NuGet

### `global.json`
```json
{
  "sdk": {
    "version": "10.0.x",
    "rollForward": "latestMinor",
    "allowPrerelease": false
  }
}
```

---

### `ProjectName` — Blazor Server Frontend

**SDK:** `Microsoft.NET.Sdk.Web`  
**Target Framework:** `net10.0`  
**Properties:** `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`

| Package | Version | Purpose |
|---------|---------|---------|
| `Blazored.LocalStorage` | 4.5.0 | Browser storage for data (JWT tokens) |
| `EPPlus` | 7.0.0 | Export data to Excel files |
| `Microsoft.AspNetCore.Authentication.OpenIdConnect` | 10.0.4 | OpenID Connect / Azure AD authentication |
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | 10.0.5 | Server-side support for WASM components |
| `Microsoft.Authentication.WebAssembly.Msal` | 10.0.4 | MSAL for WebAssembly |
| `Microsoft.Identity.Web` | 4.5.0 | Integration with Microsoft Identity Platform (Azure AD) |
| `Microsoft.Identity.Web.DownstreamApi` | 4.5.0 | Downstream API calls with token |
| `Microsoft.Identity.Web.UI` | 4.5.0 | UI components for Microsoft login |

**Project dependencies:** `ProjectName.Common`

---

### `ProjectName.Api` — ASP.NET Core REST API

**SDK:** `Microsoft.NET.Sdk.Web`  
**Target Framework:** `net10.0`  
**Properties:** `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`

| Package | Version | Purpose |
|---------|---------|---------|
| `EPPlus` | 7.0.0 | Export data to Excel (export endpoints) |
| `Swashbuckle.AspNetCore` | 7.2.0 | Swagger UI + OpenAPI documentation generation |

**Project dependencies:** `ProjectName.Database`

---

### `ProjectName.Database` — Data Layer

**SDK:** `Microsoft.NET.Sdk`  
**Target Framework:** `net10.0`  
**Properties:** `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore` | 10.0.4 | ORM — main EF Core package |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.4 | Design-time tools (migrations) — `PrivateAssets: all` |
| `Microsoft.Extensions.Configuration.Json` | 10.0.3 | Reading configuration from `appsettings.json` |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.1 | EF Core provider for PostgreSQL |

> ⚠️ **Important:** Npgsql must be version **10.0.1+** when EF Core is version **10.0.4+**.  
> Npgsql 10.0.0 depends on `Microsoft.EntityFrameworkCore.Relational` 10.0.0, which causes an assembly version conflict at runtime.

---

### `ProjectName.Common` — Shared Models

**SDK:** `Microsoft.NET.Sdk`  
**Target Framework:** `net10.0`

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Authentication.Abstractions` | 2.3.0 | Authentication abstractions |
| `Microsoft.Extensions.Logging.Abstractions` | 10.0.0 | Logging abstractions (ILogger) |

---

### `ProjectName.Tests` — Unit Tests

**SDK:** `Microsoft.NET.Sdk`  
**Target Framework:** `net10.0`  
**Properties:** `<IsPackable>false</IsPackable>`, `<IsTestProject>true</IsTestProject>`, `<LangVersion>latest</LangVersion>`

| Package | Version | Purpose |
|---------|---------|---------|
| `xunit` | 2.9.0 | Test framework |
| `xunit.runner.visualstudio` | 2.8.2 | Test runner for VS/Rider/VS Code |
| `Microsoft.NET.Test.Sdk` | 17.12.0 | .NET test infrastructure |
| `Moq` | 4.20.70 | Dependency mocking |
| `FluentAssertions` | 6.12.0 | Expressive assertions |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.4 | In-memory database for tests (isolation) |
| `coverlet.collector` | 6.0.2 | Code coverage collection |

**Project dependencies:** `ProjectName.Api`, `ProjectName.Database`

---

## 🗄️ Database

- **System:** PostgreSQL 14+
- **Application schema:** `schema_name` (equivalent to `ntrs` in NTRS 2.0)
- **ORM:** Entity Framework Core 10.0.4 with Npgsql 10.0.1
- **Initialization:** SQL scripts (schema, views, dictionary data) — EF migrations as an alternative
- **Sequences:** PostgreSQL sequences for dictionary ID generation (`nextval()`)
- **Entity configuration:** `IEntityTypeConfiguration<T>` in a separate `*.Database` project

---

## 🔐 Authentication

- **Protocol:** OpenID Connect / OAuth 2.0
- **Provider:** Azure Active Directory (Microsoft Entra ID)
- **Library:** `Microsoft.Identity.Web` 4.5.0
- **Token:** JWT stored in browser `localStorage` via `Blazored.LocalStorage`
- **Directory integration:** LDAP (optional Active Directory integration)

---

## 🧱 Architectural Patterns

| Pattern | Application |
|---------|------------|
| Layered Architecture | Separation into Frontend / API / Database |
| Repository Pattern | Data access in `*.Database` |
| Service Layer | Business logic in `*.Api/Services` |
| DTO Pattern | Data transfer objects between layers |
| Interface Segregation | Service interfaces in `/Services/Interfaces/` |
| Global Exception Handler | Middleware for error handling |
| Audit Logging | Tracking changes in the system |
| Role-Based Access Control | Permissions based on roles and groups |

---

## 🧪 Testing Strategy

- **Level:** Unit tests
- **Pattern:** AAA (Arrange, Act, Assert)
- **Database isolation:** EF Core InMemory with a unique database name per test
- **Mocking:** Moq for external dependencies
- **Assertions:** FluentAssertions
- **Coverage:** coverlet (opencover format)

---

## 📂 Project Structure Convention

```
ProjectName.Api/
├── Controllers/          # REST endpoints (one controller = one resource)
├── DTO/                  # Data Transfer Objects (request/response)
├── Services/
│   ├── Implementations/  # Service implementations
│   └── Interfaces/       # Service contracts
├── Extensions/           # Extension methods
├── Filters/              # Action filters (validation, authorization)
├── Middleware/           # Middleware (e.g. GlobalExceptionHandler)
└── Program.cs

ProjectName.Database/
├── Data/
│   └── AppDbContext.cs   # DbContext
├── Entities/             # EF Core entity classes
├── Configurations/       # IEntityTypeConfiguration<T>
├── Migrations/           # EF Core migrations
├── Repositories/         # Repositories
├── Services/             # Data layer services
└── Db init scripts/      # SQL scripts for database initialization

ProjectName/ (Blazor)
├── Components/
│   ├── Layout/           # Application layouts
│   └── Pages/            # Blazor pages (.razor)
├── Services/
│   ├── Implementations/  # HTTP services to API
│   ├── Interfaces/
│   └── Models/           # View models / frontend DTOs
└── wwwroot/              # Static files (CSS, JS, images)

ProjectName.Tests/
├── Extensions/           # Extension method tests
└── Services/             # Service tests (one file = one service)
```

---

## ⚙️ Environment Configuration

```
appsettings.json                  # Default configuration
appsettings.Development.json      # Local development environment
appsettings.Production.json       # Production environment
```

**Developer secrets** (never in repo):
```bash
dotnet user-secrets set "AzureAd:ClientSecret" "..."
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

---

## 🛠️ Developer Tools

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 10.0.x | Runtime and compilation |
| C# | 13 (latest) | Programming language |
| PostgreSQL | 14+ | Database |
| VS Code / Rider / VS 2022 | - | IDE |
| Swagger UI | via Swashbuckle 7.2.0 | API documentation (`/swagger`) |
| EF Core Tools | via `dotnet ef` | Migration management |

---

## 🚀 Running the Project (VS Code shortcuts)

The project includes ready-to-use VS Code tasks in `.vscode/tasks.json`:

| Task | Description |
|------|-------------|
| `build-api` | Builds the API project |
| `build-frontend` | Builds the Blazor project |
| `watch-api` | API with hot reload |
| `watch-frontend` | Frontend with hot reload |

---

## ⚠️ Known Pitfalls and Gotchas

1. **Npgsql vs EF Core version conflict** — Always use Npgsql `10.0.1+` with EF Core `10.0.4+`. Npgsql `10.0.0` causes a `FileNotFoundException` at runtime (`Microsoft.EntityFrameworkCore.Relational, Version=10.0.4.0`).

2. **`ValueGeneratedNever()` on dictionary entities** — If dictionary entities have `ValueGeneratedNever()` on the primary key, always provide an explicit `Id` in tests (e.g. `Id = 1`, `Id = 2`). Without an explicit `Id`, all entities default to `Id = 0`, which triggers an `InvalidOperationException` in the EF change tracker.

3. **`GetDbConnection()` only for relational databases** — Methods specific to relational providers (e.g. `GetDbConnection()`, `nextval()`) should only be called after checking `_context.Database.IsRelational()`. Otherwise, tests using the InMemory provider will throw an exception.

4. **Database initialization** — SQL scripts must be executed **before** starting the application. EF migrations do not include views and stored procedures.
