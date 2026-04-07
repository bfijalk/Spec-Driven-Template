# Skill: Create Client Service

## When to Use
When building an HTTP client service in the Blazor frontend to communicate with the API.

## System Prompt

You are implementing HTTP client services for the **Contact Manager** Blazor Server frontend. These services call the REST API and manage authentication tokens.

### Project Context:
```
ContactManager/Services/
├── Interfaces/
│   ├── IAuthClientService.cs
│   └── IContactClientService.cs
├── Implementations/
│   ├── AuthClientService.cs
│   └── ContactClientService.cs
└── Models/
```

### Registration in Program.cs:
```csharp
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001/";
builder.Services.AddHttpClient<IAuthClientService, AuthClientService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IContactClientService, ContactClientService>(client =>
    client.BaseAddress = new Uri(apiBaseUrl));
```

### Service Pattern:
```csharp
public class ContactClientService : IContactClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ContactClientService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _localStorage.GetItemAsStringAsync("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            token = token.Trim('"');
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<IEnumerable<ContactDto>> GetAllAsync(string? query = null)
    {
        await SetAuthHeaderAsync();
        var url = string.IsNullOrWhiteSpace(query) ? "api/contacts" : $"api/contacts?q={query}";
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ContactDto>>>(url);
        return response?.Data ?? [];
    }
}
```

### Rules:
1. **Always call `SetAuthHeaderAsync()`** before every API request (JWT from localStorage)
2. **Use typed `HttpClient`** — registered via `AddHttpClient<IService, Implementation>()` in DI
3. **Deserialize to `ApiResponse<T>`** — check `.Success` and `.Data`
4. **Token storage:** Use `Blazored.LocalStorage` (`ILocalStorageService`)
5. **Error handling:** Catch `HttpRequestException`, return sensible defaults or rethrow
6. **One service per API controller** — `IContactClientService` ↔ `ContactsController`, `IAuthClientService` ↔ `AuthController`
7. **Interface in Interfaces/**, implementation in **Implementations/**
8. **All methods async** — return `Task<T>`
9. **Use `System.Net.Http.Json`** methods: `GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`
10. **API base URL** comes from `HttpClient.BaseAddress` (configured in DI)

## Input Expected
- API endpoints to call (method + route + request/response types)
- Whether auth is required
- Any special error handling needs

## Output
- Interface (`IXxxClientService.cs`)
- Implementation (`XxxClientService.cs`)
- DI registration line for `Program.cs`
