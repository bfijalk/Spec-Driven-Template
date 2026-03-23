using ContactManager.Api.Services.Implementations;
using ContactManager.Common.DTOs;
using ContactManager.Database.Entities;
using ContactManager.Database.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace ContactManager.Tests.Services;

public class AuthServiceTests
{
    private IConfiguration BuildConfig(string jwtKey = "supersecretkey_for_testing_purposes_only_32chars")
    {
        var inMemory = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = jwtKey,
            ["Jwt:Issuer"] = "ContactManager",
            ["Jwt:Audience"] = "ContactManager",
            ["Jwt:ExpiresHours"] = "24"
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();
    }

    // -----------------------------------------------------------------------
    // Register
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Register_WithValidData_ReturnsTokenAndEmail()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        var request = new RegisterRequest
        {
            Email = "alice@example.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!"
        };

        // Act
        var result = await sut.RegisterAsync(request);

        // Assert
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Email.Should().Be("alice@example.com");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        var request = new RegisterRequest
        {
            Email = "bob@example.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!"
        };
        await sut.RegisterAsync(request);

        // Act
        var act = () => sut.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task Register_StoresPasswordHash_NotPlaintext()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        // Act
        await sut.RegisterAsync(new RegisterRequest
        {
            Email = "carol@example.com",
            Password = "PlainTextPass1!",
            ConfirmPassword = "PlainTextPass1!"
        });

        // Assert
        var user = await repo.GetByEmailAsync("carol@example.com");
        user!.PasswordHash.Should().NotBe("PlainTextPass1!");
        user.PasswordHash.Should().StartWith("$2");   // BCrypt hash prefix
    }

    // -----------------------------------------------------------------------
    // Login
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        await sut.RegisterAsync(new RegisterRequest
        {
            Email = "dave@example.com",
            Password = "CorrectHorse1!",
            ConfirmPassword = "CorrectHorse1!"
        });

        // Act
        var result = await sut.LoginAsync(new LoginRequest
        {
            Email = "dave@example.com",
            Password = "CorrectHorse1!"
        });

        // Assert
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.Email.Should().Be("dave@example.com");
    }

    [Fact]
    public async Task Login_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        await sut.RegisterAsync(new RegisterRequest
        {
            Email = "eve@example.com",
            Password = "RightPass1!",
            ConfirmPassword = "RightPass1!"
        });

        // Act
        var act = () => sut.LoginAsync(new LoginRequest
        {
            Email = "eve@example.com",
            Password = "WrongPass!"
        });

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Invalid email or password*");
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var repo = new UserRepository(ctx);
        var sut = new AuthService(repo, BuildConfig());

        // Act
        var act = () => sut.LoginAsync(new LoginRequest
        {
            Email = "nobody@example.com",
            Password = "AnyPass1!"
        });

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
