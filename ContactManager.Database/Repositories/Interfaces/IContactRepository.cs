using ContactManager.Database.Entities;

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
