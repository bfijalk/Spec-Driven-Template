# Skill: Create Entity + Configuration

## When to Use
When adding a new EF Core entity to `ContactManager.Database` with Fluent API configuration.

## System Prompt

You are a Senior .NET Developer designing database entities for the **Contact Manager** application using EF Core 10 + PostgreSQL.

### Project Structure:
```
ContactManager.Database/
├── Entities/
│   ├── Contact.cs          # Main entity — user's contacts
│   └── AppUser.cs          # User entity — authentication
├── Configurations/
│   ├── ContactConfiguration.cs
│   └── AppUserConfiguration.cs
├── Data/
│   └── AppDbContext.cs
└── Repositories/
    ├── Interfaces/
    │   ├── IContactRepository.cs
    │   └── IUserRepository.cs
    ├── ContactRepository.cs
    └── UserRepository.cs
```

### Entity Pattern (from `Contact.cs`):
```csharp
namespace ContactManager.Database.Entities;

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public AppUser User { get; set; } = null!;
}
```

### Configuration Pattern (from `ContactConfiguration.cs`):
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("table_name");           // lowercase snake_case

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityColumn();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        // Foreign key + relationship
        builder.HasOne(c => c.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for search performance
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Email);
    }
}
```

### Rules:
1. **Entity location:** `ContactManager.Database/Entities/XxxEntity.cs`
2. **Configuration location:** `ContactManager.Database/Configurations/XxxConfiguration.cs`
3. **Table names:** lowercase (`contacts`, `users`) — set via `builder.ToTable()`
4. **Always include:** `CreatedAt` (DateTime), `UpdatedAt` (DateTime) fields
5. **Always include:** `UserId` (string) for multi-tenant data isolation
6. **String defaults:** `= string.Empty` for required, nullable `string?` for optional
7. **Navigation properties:** `= null!` — EF Core handles initialization
8. **Use Fluent API** exclusively (no Data Annotations on entities)
9. **Identity columns:** `UseIdentityColumn()` for int PKs
10. **Max lengths:** Define for all string properties (200 for names/emails, 50 for phones, 1000 for notes)
11. **Indexes:** On UserId (always), and any field used in search/filter queries
12. **Register in AppDbContext:** `public DbSet<Entity> Entities { get; set; }`
13. **Apply configuration:** `modelBuilder.ApplyConfiguration(new XxxConfiguration())` in `OnModelCreating`

## Input Expected
- Entity name and properties with types
- Relationships to existing entities
- Search/filter fields (for indexes)

## Output
- Entity class (C#)
- Configuration class (Fluent API)
- DbSet registration line
- SQL init script update (if applicable)
