# Skill: Create Repository

## When to Use
When implementing a new repository in `ContactManager.Database/Repositories/` for data access.

## System Prompt

You are implementing the data access layer for the **Contact Manager** application using the Repository pattern with EF Core 10.

### Repository Pattern (from `ContactRepository.cs`):

**Interface:**
```csharp
// ContactManager.Database/Repositories/Interfaces/IContactRepository.cs
namespace ContactManager.Database.Repositories.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllByUserIdAsync(string userId);
    Task<IEnumerable<Contact>> SearchAsync(string userId, string query);
    Task<Contact?> GetByIdAsync(int id, string userId);
    Task<Contact> CreateAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(Contact contact);
}
```

**Implementation:**
```csharp
// ContactManager.Database/Repositories/ContactRepository.cs
public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Contact>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Contacts
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> SearchAsync(string userId, string query)
    {
        var lower = query.ToLower();
        return await _context.Contacts
            .Where(c => c.UserId == userId &&
                (c.Name.ToLower().Contains(lower) ||
                 (c.Email != null && c.Email.ToLower().Contains(lower)) ||
                 (c.Phone != null && c.Phone.ToLower().Contains(lower))))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(int id, string userId)
    {
        return await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task DeleteAsync(Contact contact)
    {
        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
    }
}
```

### Rules:
1. **Interface** in `Repositories/Interfaces/IXxxRepository.cs`
2. **Implementation** in `Repositories/XxxRepository.cs`
3. **Constructor injection** of `AppDbContext` — single dependency
4. **All methods async** — use `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`
5. **Always filter by UserId** — multi-tenant data isolation
6. **Default ordering** — `OrderBy(Name)` for list queries
7. **Search pattern:** `ToLower().Contains()` for case-insensitive partial match
8. **Null handling** in search: check nullable fields with `!= null &&` before comparing
9. **Return entities** — mapping to DTOs happens in the service layer
10. **No business logic** — repositories are pure data access
11. **Register in Program.cs:** `builder.Services.AddScoped<IXxxRepository, XxxRepository>()`

## Input Expected
- Entity name
- Required CRUD operations
- Search/filter fields
- Any special queries (pagination, aggregation)

## Output
- Interface definition
- Implementation class
- DI registration line
