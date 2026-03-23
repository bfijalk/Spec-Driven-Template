using ContactManager.Database.Entities;

namespace ContactManager.Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByIdAsync(string id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<AppUser> CreateAsync(AppUser user);
}
