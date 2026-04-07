# Skill: Create Service Layer

## When to Use
When implementing business logic in the service layer of `ContactManager.Api`.

## System Prompt

You are implementing service classes for the **Contact Manager** API. Services sit between controllers and repositories, containing all business logic.

### Architecture:
```
Controller → Service (business logic) → Repository (data access) → EF Core → PostgreSQL
```

### Existing Service Pattern (from `ContactService`):

```csharp
// Interface — in ContactManager.Api/Services/Interfaces/
public interface IContactService
{
    Task<IEnumerable<ContactDto>> GetAllAsync(string userId);
    Task<ContactDto?> GetByIdAsync(int id, string userId);
    Task<ContactDto> CreateAsync(CreateContactRequest request, string userId);
    Task<ContactDto?> UpdateAsync(int id, UpdateContactRequest request, string userId);
    Task<bool> DeleteAsync(int id, string userId);
    Task<IEnumerable<ContactDto>> SearchAsync(string userId, string query);
}

// Implementation — in ContactManager.Api/Services/Implementations/
public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    
    public ContactService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }
    
    // Maps Entity → DTO (private helper)
    private static ContactDto MapToDto(Contact contact) => new()
    {
        Id = contact.Id,
        Name = contact.Name,
        // ...
    };
}
```

### Rules:
1. **Services NEVER return entities** — always map to DTOs
2. **UserId is always passed** from controller — services verify ownership
3. **Throw exceptions** for business rule violations — `GlobalExceptionHandler` catches them
4. **Single Responsibility** — one service per domain concept
5. **All methods are async** — return `Task<T>`
6. **Null return = not found** — controller maps to 404
7. **Use private static mapper methods** — `MapToDto()` pattern
8. **Constructor injection** for repositories only — no direct DbContext access
9. **No HTTP concepts** — services don't know about HttpContext, StatusCodes, etc.
10. **Validation** — validate business rules (e.g., duplicate check), not input format (that's the controller/DTO's job)

### Common Patterns:
```csharp
// Ownership check
var entity = await _repository.GetByIdAsync(id);
if (entity is null || entity.UserId != userId) return null;

// Create with mapping
var entity = new Contact
{
    Name = request.Name,
    UserId = userId,
    CreatedAt = DateTime.UtcNow
};
await _repository.AddAsync(entity);
return MapToDto(entity);
```

## Input Expected
- Domain concept / entity name
- Required operations (CRUD + any special business logic)
- Related repository interface
- Related DTOs

## Output
- `IXxxService` interface with all method signatures
- `XxxService` implementation with full business logic
- Private mapper methods
- DI registration line
