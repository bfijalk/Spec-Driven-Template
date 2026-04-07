# Skill: Create DTO

## When to Use
When defining new Data Transfer Objects in `ContactManager.Common/DTOs/`.

## System Prompt

You are creating DTOs for the **Contact Manager** application. DTOs are shared between the API and Blazor frontend via the `ContactManager.Common` project.

### Existing DTO Patterns:

```csharp
// Response DTO — returned from API
public class ContactDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public string UserId { get; set; } = string.Empty;
}

// Create Request DTO — sent to API for creation
public class CreateContactRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Phone]
    public string? Phone { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    public string? Notes { get; set; }
}

// Generic API Response wrapper
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    
    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(string error) => new() { Success = false, Error = error };
}
```

### Rules:
1. **Location:** All DTOs in `ContactManager.Common/DTOs/` — one file per DTO
2. **Naming convention:**
   - Response DTOs: `XxxDto` (e.g., `ContactDto`)
   - Create requests: `CreateXxxRequest`
   - Update requests: `UpdateXxxRequest`
   - Auth: `LoginRequest`, `RegisterRequest`, `AuthResponse`
3. **Use Data Annotations** for validation on request DTOs: `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Phone]`
4. **String defaults:** `= string.Empty` for required strings
5. **Nullable:** Use `string?` for optional fields
6. **No business logic** in DTOs — they are pure data containers
7. **No entity references** — DTOs never reference `ContactManager.Database`
8. **Keep flat** — avoid nested complex objects unless truly needed
9. **All API responses** must use `ApiResponse<T>` wrapper

### DTO Types to Consider:
- **Response DTO** (`XxxDto`) — what the API returns
- **Create Request** (`CreateXxxRequest`) — what the client sends to create
- **Update Request** (`UpdateXxxRequest`) — what the client sends to update (often same fields as Create)
- **List/Filter Request** — query parameters for search/filter endpoints

## Input Expected
- Entity or feature description
- Fields with types and constraints
- Whether it's a request or response DTO

## Output
- Complete C# DTO class with namespace, using statements, and data annotations
- File path: `ContactManager.Common/DTOs/[ClassName].cs`
