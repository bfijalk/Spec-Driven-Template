# Implementation Plan: Contact Management Application

## Overview

Build a **Contact Management Blazor Server application from scratch** using the defined tech stack (Blazor Server frontend, ASP.NET Core REST API, EF Core + PostgreSQL, Azure AD auth). The plan follows the 5-project solution structure and covers all 7 user stories (US_001–US_007).

**Tech Stack Reference:** [tech-stack.md](Tech/tech-stack.md)  
**User Stories:** [stories/](stories/)

---

## Solution Structure

```
ContactManager/
├── ContactManager/              # Frontend — Blazor Server
├── ContactManager.Api/          # Backend — ASP.NET Core REST API
├── ContactManager.Database/     # Data layer — EF Core + PostgreSQL
├── ContactManager.Common/       # Shared models/DTOs
└── ContactManager.Tests/        # Unit tests
```

---

## Implementation Steps

### Step 1 — Scaffold the Solution

Create `ContactManager.sln` with 5 projects and configure all dependencies.

**Tasks:**
- Create solution file and 5 `.csproj` projects with correct SDKs and `net10.0` target
- Add `global.json` with SDK version `10.0.x`, `rollForward: latestMinor`
- Install all NuGet packages per tech-stack.md (including Npgsql `10.0.1`)
- Wire up project references:
  - `ContactManager` → `ContactManager.Common`
  - `ContactManager.Api` → `ContactManager.Database`
  - `ContactManager.Database` → _(no internal deps)_
  - `ContactManager.Common` → _(no internal deps)_
  - `ContactManager.Tests` → `ContactManager.Api`, `ContactManager.Database`
- Add `.vscode/tasks.json` with `build-api`, `build-frontend`, `watch-api`, `watch-frontend` tasks
- Add `appsettings.json` / `appsettings.Development.json` for both API and frontend projects

**Covers:** Foundation for all user stories

---

### Step 2 — Shared Models (`ContactManager.Common`)

Define all DTOs and shared contracts used across the API and frontend.

**Tasks:**
- `ContactDto` — `Id`, `Name`, `Phone`, `Email`, `Notes`, `UserId`
- `CreateContactRequest` — `Name` (required), `Phone`, `Email`, `Notes`
- `UpdateContactRequest` — same fields as create
- `LoginRequest` — `Email`, `Password`
- `RegisterRequest` — `Email`, `Password`, `ConfirmPassword`
- `AuthResponse` — `Token`, `Email`, `ExpiresAt`
- `ApiResponse<T>` — generic wrapper for API responses

**Covers:** US_001 (auth DTOs), US_002–US_005 (contact DTOs)

---

### Step 3 — Data Layer (`ContactManager.Database`)

Build the database layer with EF Core, PostgreSQL, and the repository pattern.

**Tasks:**
- `Contact` entity class with properties: `Id`, `Name`, `Phone`, `Email`, `Notes`, `UserId`, `CreatedAt`, `UpdatedAt`
- `AppUser` entity class: `Id`, `Email`, `PasswordHash`, `CreatedAt`
- `AppDbContext` with `contacts` schema configuration
- `IEntityTypeConfiguration<Contact>` and `IEntityTypeConfiguration<AppUser>` (Fluent API)
- `IContactRepository` interface + `ContactRepository` implementation
- `IUserRepository` interface + `UserRepository` implementation
- EF Core migration (initial schema)
- SQL init script in `Db init scripts/` as alternative
- Database indexes on `UserId`, `Name`, `Email`, `Phone` (for search performance — US_006)

**Covers:** US_002 (contact data model), US_003–US_005 (CRUD DB ops), US_006 (search indexes)

---

### Step 4 — REST API (`ContactManager.Api`)

Implement all backend endpoints, services, middleware, and Swagger docs.

**Tasks:**

#### Controllers
- `AuthController`
  - `POST /api/auth/register` — register with email + password (BCrypt hashing)
  - `POST /api/auth/login` — validate credentials, return JWT
- `ContactsController` _(requires auth)_
  - `GET /api/contacts` — get all contacts for authenticated user (sorted by name)
  - `GET /api/contacts/{id}` — get single contact (ownership verified)
  - `POST /api/contacts` — create new contact
  - `PUT /api/contacts/{id}` — update existing contact (ownership verified)
  - `DELETE /api/contacts/{id}` — delete contact (ownership verified)
  - `GET /api/contacts?q={query}` — search contacts by name/phone/email (case-insensitive, partial match)

#### Services
- `IAuthService` / `AuthService` — registration, login, JWT generation
- `IContactService` / `ContactService` — business logic for all CRUD + search operations

#### Infrastructure
- `GlobalExceptionHandler` middleware — catch unhandled exceptions, return structured error responses
- JWT authentication middleware configuration
- `Program.cs` — DI registration, Swagger, CORS, middleware pipeline
- Swagger/OpenAPI configuration (accessible at `/swagger`)

**Covers:** US_001 (auth endpoints), US_002 (GET contacts), US_003 (POST), US_004 (PUT), US_005 (DELETE), US_006 (search query param)

---

### Step 5 — Blazor Server Frontend (`ContactManager`)

Build all Blazor pages, components, and HTTP client services.

**Tasks:**

#### Pages
- `Login.razor` — email/password form, error messages, redirect to dashboard on success
- `Register.razor` — registration form with client-side validation (email format, password strength), success redirect
- `Dashboard.razor` — contact list with search box, empty state, loading state, add/edit/delete actions
- `AddContact.razor` — form with name (required), phone, email, notes; validation; cancel/save navigation
- `EditContact.razor` — pre-populated form loaded from API; update on save; cancel returns without changes

#### Components
- `ContactCard.razor` — single contact row/card with edit + delete buttons
- `DeleteConfirmationDialog.razor` — modal with contact name, confirm/cancel buttons
- `SearchInput.razor` — debounced search input with clear button
- `LoadingSpinner.razor` — reusable loading indicator
- `EmptyState.razor` — illustrated empty state with CTA

#### Services
- `IAuthService` / `AuthService` — HTTP calls to auth endpoints, JWT storage via `Blazored.LocalStorage`
- `IContactService` / `ContactService` — HTTP calls to all contact endpoints
- `AuthStateProvider` — custom `AuthenticationStateProvider` reading JWT from localStorage

#### Configuration
- Azure AD OIDC setup in `Program.cs` (via `Microsoft.Identity.Web`)
- `HttpClient` configured with base URL and auth header injection
- Route guards for authenticated pages

**Covers:** US_001 (login/register/logout), US_002 (dashboard), US_003 (add), US_004 (edit), US_005 (delete + confirmation), US_006 (search)

---

### Step 6 — Responsive Design

Apply CSS media queries and responsive layout for mobile, tablet, and desktop.

**Tasks:**
- Define breakpoints in `wwwroot/css/app.css`:
  - Mobile: `< 768px`
  - Tablet: `768px – 1023px`
  - Desktop: `≥ 1024px`
- Responsive navigation — hamburger menu / simplified layout on mobile; logout accessible on all sizes
- Contact list — multi-column cards on desktop, single-column stacked layout on mobile
- Forms — appropriately sized fields, `type="email"` and `type="tel"` for correct mobile keyboards, tappable buttons
- Search input — full-width on mobile, constrained on desktop
- Touch targets — minimum 44×44px for all interactive elements

**Covers:** US_007 (responsive design — all subtasks)

---

### Step 7 — Unit Tests (`ContactManager.Tests`)

Write comprehensive unit tests following AAA pattern with EF Core InMemory for isolation.

**Tasks:**
- `AuthServiceTests.cs` — register (valid, duplicate email, weak password), login (valid, invalid credentials)
- `ContactServiceTests.cs` — get all (sorted), get by id (found, not found, wrong user), create (valid, missing name), update (valid, ownership check), delete (valid, not found)
- `ContactsControllerTests.cs` — HTTP response codes, model binding, authorization
- `AuthControllerTests.cs` — HTTP response codes, validation
- Each test class uses a unique in-memory DB name
- `ValueGeneratedNever()` entities always get explicit `Id` values in test setup

**Covers:** US_001–US_006 testing subtasks (ST_007 in each story)

---

## Open Decisions

> These decisions should be confirmed before implementation begins.

| # | Question | Options |
|---|----------|---------|
| 1 | **Authentication approach** | **A** — Azure AD OIDC only (no local registration) · **B** — Local JWT with BCrypt (matches US_001 ACs) · **C** — Both |
| 2 | **Search implementation** | **A** — Dedicated `GET /api/contacts/search?q=` endpoint · **B** — Query param on `GET /api/contacts?q=` (RESTful, simpler — recommended) |
| 3 | **DB initialization** | **A** — EF Core migrations only · **B** — SQL init scripts only · **C** — Both (migrations for dev, scripts for prod) |

---

## User Story Coverage Map

| Story | Step(s) |
|-------|---------|
| US_001 — User Authentication | 2, 4, 5, 7 |
| US_002 — Contact Dashboard | 2, 3, 4, 5, 7 |
| US_003 — Add New Contact | 2, 3, 4, 5, 7 |
| US_004 — Edit Contact | 2, 3, 4, 5, 7 |
| US_005 — Delete Contact | 2, 3, 4, 5, 7 |
| US_006 — Search & Filter | 3, 4, 5, 7 |
| US_007 — Responsive Design | 6 |
