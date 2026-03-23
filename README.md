# Contact Manager

A full-stack **Contact Management** web application built with **.NET 10**, **Blazor Server**, and **PostgreSQL**. Designed as a Spec-Driven Development template demonstrating AI-assisted SDLC from user stories through to a working application.

---

## 🏗️ Architecture

```
ContactManager.slnx
├── ContactManager          # Blazor Server frontend (UI)
├── ContactManager.Api      # ASP.NET Core REST API (backend)
├── ContactManager.Database # EF Core data layer (entities, repositories, migrations)
├── ContactManager.Common   # Shared DTOs used by both frontend and API
└── ContactManager.Tests    # xUnit unit tests
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Blazor Server (.NET 10), Bootstrap 5 |
| Backend | ASP.NET Core Web API (.NET 10) |
| Auth | JWT Bearer tokens + BCrypt password hashing |
| ORM | Entity Framework Core 10 + Npgsql |
| Database | PostgreSQL 14+ |
| API Docs | Scalar UI (`Microsoft.AspNetCore.OpenApi`) |
| Tests | xUnit, Moq, FluentAssertions, EF InMemory |
| Storage | `Blazored.LocalStorage` (JWT token persistence) |

---

## ✅ Features

- 🔐 **User Authentication** — Register, Login, JWT-secured sessions
- 📋 **Contact Dashboard** — Paginated list of all contacts
- ➕ **Add Contact** — Create new contacts with full validation
- ✏️ **Edit Contact** — Inline editing of contact details
- 🗑️ **Delete Contact** — Confirmation dialog before deletion
- 🔍 **Search & Filter** — Real-time contact search by name/email
- 📱 **Responsive Design** — Mobile-first Bootstrap 5 layout

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 14+](https://www.postgresql.org/download/)

### 1. Clone the repository

```bash
git clone git@github.com:bfijalk/Spec-Driven-Template.git
cd Spec-Driven-Template
```

### 2. Set up the database

Connect to PostgreSQL and run the init script:

```bash
psql -U postgres -f "ContactManager.Database/Db init scripts/init.sql"
```

This creates:
- Database: `contactmanager`
- Schema: `contacts`
- Tables: `users`, `contacts` with indexes
- Seed data: 1 admin user + 5 sample contacts

### 3. Configure the API

Edit `ContactManager.Api/appsettings.json` if your PostgreSQL setup differs:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=contactmanager;Username=postgres"
  },
  "Jwt": {
    "Key": "ContactManager$SecretKey#2026!LocalDev@32chars"
  }
}
```

### 4. Run the API

```bash
cd ContactManager.Api
dotnet run
```

API listens on:
- `https://localhost:7100`
- `http://localhost:5300`

Scalar API UI: **https://localhost:7100/scalar/v1**

### 5. Run the Blazor frontend

Open a new terminal:

```bash
cd ContactManager
dotnet run
```

Frontend available at:
- `https://localhost:7200`
- `http://localhost:5400`

---

## 🔑 Default Credentials

| Field | Value |
|-------|-------|
| Email | `admin@demo.com` |
| Password | `Admin1234!` |

---

## 🧪 Running Tests

```bash
dotnet test
```

**20 tests** — all passing:
- `AuthServiceTests` (6 tests) — registration, login, duplicate detection
- `ContactServiceTests` (14 tests) — CRUD, search, authorization

---

## 📁 Project Structure Details

```
ContactManager/
├── Components/
│   ├── Pages/          # Login, Register, Dashboard, AddContact, EditContact
│   ├── Layout/         # MainLayout, NavMenu
│   └── Shared/         # ContactCard, DeleteConfirmationDialog, LoadingSpinner
└── Services/           # AuthClientService, ContactClientService (HTTP clients)

ContactManager.Api/
├── Controllers/        # AuthController, ContactsController
├── Services/           # AuthService, ContactService (business logic)
└── Middleware/         # GlobalExceptionHandler

ContactManager.Database/
├── Entities/           # AppUser, Contact
├── Configurations/     # EF Fluent API configs
├── Repositories/       # IContactRepository, IUserRepository + implementations
└── Db init scripts/    # init.sql (schema + seed data)

ContactManager.Common/
└── DTOs/               # LoginRequest, RegisterRequest, ContactDto, ApiResponse, etc.

AI/
├── stories/            # EPIC + 7 User Stories (Spec-Driven source)
└── Tech/               # tech-stack.md
```

---

## 🗄️ Database Schema

```sql
Schema: contacts

Table: users
  - id          UUID PRIMARY KEY
  - email       VARCHAR(255) UNIQUE NOT NULL
  - password_hash VARCHAR(255) NOT NULL
  - created_at  TIMESTAMP

Table: contacts
  - id          UUID PRIMARY KEY
  - user_id     UUID REFERENCES users(id)
  - first_name  VARCHAR(100)
  - last_name   VARCHAR(100)
  - email       VARCHAR(255)
  - phone       VARCHAR(50)
  - company     VARCHAR(150)
  - notes       TEXT
  - created_at  TIMESTAMP
  - updated_at  TIMESTAMP
```

---

## 📖 Spec-Driven Development

This project was built following a **Spec-Driven SDLC** workflow:

1. **Epic** defined in `AI/stories/EPIC_contact-management.md`
2. **7 User Stories** written in `AI/stories/US_00X_*.md`
3. **Tech stack** specified in `AI/Tech/tech-stack.md`
4. **Code generated** iteratively by AI (GitHub Copilot) from the specs
5. **Bugs fixed** and **tests written** as part of the cycle

---

## 📄 License

MIT
