using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContactManager.Common.DTOs;
using ContactManager.Services.Interfaces;

namespace ContactManager.Services.Implementations;

public class ContactClientService : IContactClientService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthClientService _authService;

    public ContactClientService(HttpClient httpClient, IAuthClientService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IEnumerable<ContactDto>> GetAllAsync(string? searchQuery = null)
    {
        await SetAuthHeaderAsync();
        var url = string.IsNullOrWhiteSpace(searchQuery)
            ? "api/contacts"
            : $"api/contacts?q={Uri.EscapeDataString(searchQuery)}";

        var result = await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<ContactDto>>>(url);
        return result?.Data ?? Enumerable.Empty<ContactDto>();
    }

    public async Task<ContactDto?> GetByIdAsync(int id)
    {
        await SetAuthHeaderAsync();
        var result = await _httpClient.GetFromJsonAsync<ApiResponse<ContactDto>>($"api/contacts/{id}");
        return result?.Data;
    }

    public async Task<ContactDto?> CreateAsync(CreateContactRequest request)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("api/contacts", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        return result?.Data;
    }

    public async Task<ContactDto?> UpdateAsync(int id, UpdateContactRequest request)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.PutAsJsonAsync($"api/contacts/{id}", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        return result?.Data;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await SetAuthHeaderAsync();
        var response = await _httpClient.DeleteAsync($"api/contacts/{id}");
        return response.IsSuccessStatusCode;
    }
}
