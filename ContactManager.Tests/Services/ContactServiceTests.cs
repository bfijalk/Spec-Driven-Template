using ContactManager.Api.Services.Implementations;
using ContactManager.Common.DTOs;
using ContactManager.Database.Entities;
using ContactManager.Database.Repositories;
using FluentAssertions;

namespace ContactManager.Tests.Services;

public class ContactServiceTests
{
    private static AppUser MakeUser(string id = "user-1") => new()
    {
        Id = id,
        Email = $"{id}@example.com",
        PasswordHash = "hash",
        CreatedAt = DateTime.UtcNow
    };

    private static async Task<(ContactService sut, string userId)> SetupAsync(
        ContactManager.Database.Data.AppDbContext ctx,
        string userId = "user-1")
    {
        ctx.Users.Add(MakeUser(userId));
        await ctx.SaveChangesAsync();

        var repo = new ContactRepository(ctx);
        return (new ContactService(repo), userId);
    }

    // -----------------------------------------------------------------------
    // GetAll
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetAll_ReturnsOnlyCurrentUserContacts_SortedByName()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        ctx.Users.Add(MakeUser("user-2"));
        await ctx.SaveChangesAsync();

        ctx.Contacts.AddRange(
            new Contact { Name = "Zara",  UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Contact { Name = "Alice", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Contact { Name = "Bob",   UserId = "user-2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = (await sut.GetAllAsync("user-1")).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Alice");
        result[1].Name.Should().Be("Zara");
    }

    [Fact]
    public async Task GetAll_WhenNoContacts_ReturnsEmptyList()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var (sut, userId) = await SetupAsync(ctx);

        // Act
        var result = await sut.GetAllAsync(userId);

        // Assert
        result.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // Search
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Search_MatchesPartialNameCaseInsensitive()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.AddRange(
            new Contact { Name = "Jonathan Smith", Email = "j@example.com", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Contact { Name = "Maria Garcia",   Email = "m@example.com", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = (await sut.SearchAsync("user-1", "jonATH")).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Jonathan Smith");
    }

    [Fact]
    public async Task Search_MatchesByEmail()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Name = "Test User", Email = "unique@domain.com", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = (await sut.SearchAsync("user-1", "unique@domain")).ToList();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Search_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var (sut, userId) = await SetupAsync(ctx);

        // Act
        var result = await sut.SearchAsync(userId, "xyznotfound");

        // Assert
        result.Should().BeEmpty();
    }

    // -----------------------------------------------------------------------
    // GetById
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetById_ReturnsContact_WhenOwnedByUser()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "Found", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.GetByIdAsync(1, "user-1");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Found");
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenContactBelongsToDifferentUser()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        ctx.Users.Add(MakeUser("user-2"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "Private", UserId = "user-2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.GetByIdAsync(1, "user-1");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenContactDoesNotExist()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var (sut, userId) = await SetupAsync(ctx);

        // Act
        var result = await sut.GetByIdAsync(999, userId);

        // Assert
        result.Should().BeNull();
    }

    // -----------------------------------------------------------------------
    // Create
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Create_WithValidData_PersistsAndReturnsDto()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var (sut, userId) = await SetupAsync(ctx);

        var request = new CreateContactRequest
        {
            Name = "New Contact",
            Phone = "123",
            Email = "new@example.com",
            Notes = "Some note"
        };

        // Act
        var result = await sut.CreateAsync(request, userId);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("New Contact");
        result.UserId.Should().Be(userId);
        ctx.Contacts.Should().HaveCount(1);
    }

    // -----------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Update_WithValidOwnership_UpdatesFields()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "Old Name", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.UpdateAsync(1, new UpdateContactRequest { Name = "New Name", Email = "updated@example.com" }, "user-1");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("New Name");
        result.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenContactNotOwnedByUser()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        ctx.Users.Add(MakeUser("user-2"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "Owned by 2", UserId = "user-2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.UpdateAsync(1, new UpdateContactRequest { Name = "Hack" }, "user-1");

        // Assert
        result.Should().BeNull();
    }

    // -----------------------------------------------------------------------
    // Delete
    // -----------------------------------------------------------------------

    [Fact]
    public async Task Delete_WithValidOwnership_RemovesContactAndReturnsTrue()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "To Delete", UserId = "user-1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.DeleteAsync(1, "user-1");

        // Assert
        result.Should().BeTrue();
        ctx.Contacts.Should().BeEmpty();
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenContactNotFound()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        var (sut, userId) = await SetupAsync(ctx);

        // Act
        var result = await sut.DeleteAsync(999, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenContactBelongsToDifferentUser()
    {
        // Arrange
        using var ctx = TestDbContextFactory.Create();
        ctx.Users.Add(MakeUser("user-1"));
        ctx.Users.Add(MakeUser("user-2"));
        await ctx.SaveChangesAsync();
        ctx.Contacts.Add(new Contact { Id = 1, Name = "Protected", UserId = "user-2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await ctx.SaveChangesAsync();

        var sut = new ContactService(new ContactRepository(ctx));

        // Act
        var result = await sut.DeleteAsync(1, "user-1");

        // Assert
        result.Should().BeFalse();
        ctx.Contacts.Should().HaveCount(1);
    }
}
