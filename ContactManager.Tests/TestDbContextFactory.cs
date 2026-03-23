using ContactManager.Database.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Tests;

/// <summary>
/// Creates a unique in-memory AppDbContext per test to ensure full isolation.
/// </summary>
public static class TestDbContextFactory
{
    public static AppDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
