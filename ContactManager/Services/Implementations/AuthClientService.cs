using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using ContactManager.Common.DTOs;
using ContactManager.Services.Interfaces;

namespace ContactManager.Services.Implementations;

public class AuthClientService : IAuthClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "auth_token";

    public AuthClientService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        if (result?.Data is not null)
            await _localStorage.SetItemAsync(TokenKey, result.Data.Token);

        return result?.Data;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        if (result?.Data is not null)
            await _localStorage.SetItemAsync(TokenKey, result.Data.Token);

        return result?.Data;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}
