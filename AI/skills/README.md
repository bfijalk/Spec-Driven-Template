# 🧠 AI Skills for Spec-Driven Development

Reusable AI prompts/skills tailored to the **Contact Manager** project.  
Use these as system instructions or chat context to get consistent, high-quality AI output aligned with the project's architecture and conventions.

---

## 📁 Skill Categories

| Folder | Purpose |
|--------|---------|
| [`planning/`](planning/) | Spec-driven planning — epics, user stories, implementation plans |
| [`backend/`](backend/) | ASP.NET Core API — controllers, services, middleware |
| [`frontend/`](frontend/) | Blazor Server — pages, components, client services |
| [`database/`](database/) | EF Core + PostgreSQL — entities, repositories, migrations |
| [`testing/`](testing/) | xUnit tests — unit, integration, controller tests |
| [`review/`](review/) | Code review, quality checks, security audit |

---

## 🚀 How to Use

1. **Pick the skill** matching your current task
2. **Attach as context** — paste the skill content into your AI chat, or reference the file
3. **Provide input** — the skill will tell you what input it expects (e.g., a user story, a DTO, a controller)
4. **Iterate** — use the output, refine, and chain skills together

### Example Workflow: Implementing a New Feature

```
1. planning/write-user-story.md    → Write the user story from a feature idea
2. planning/break-into-subtasks.md → Break user story into subtasks
3. database/create-entity.md       → Design EF Core entity + configuration
4. backend/create-api-endpoint.md  → Implement the API controller + service
5. frontend/create-blazor-page.md  → Build the Blazor page
6. testing/write-unit-tests.md     → Write unit tests for the service layer
7. testing/write-controller-tests.md → Write tests for the controller
8. review/code-review.md           → AI-assisted code review
```

---

## 📏 Project Conventions (Shared Context)

All skills assume the following project context:

- **Framework:** .NET 10, C# 14
- **Frontend:** Blazor Server with Interactive Server rendering
- **Backend:** ASP.NET Core REST API with JWT auth
- **Database:** PostgreSQL + EF Core 10 + Npgsql
- **Testing:** xUnit + Moq + FluentAssertions + EF InMemory
- **Solution structure:** 5 projects (`ContactManager`, `.Api`, `.Database`, `.Common`, `.Tests`)
- **Patterns:** Repository pattern, service layer, DTOs in Common, `ApiResponse<T>` wrapper
- **Auth:** JWT Bearer tokens + BCrypt password hashing
- **Naming:** PascalCase for C#, kebab-case for routes, snake_case for DB columns
