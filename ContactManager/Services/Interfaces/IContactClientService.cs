using ContactManager.Common.DTOs;

namespace ContactManager.Services.Interfaces;

public interface IContactClientService
{
    Task<IEnumerable<ContactDto>> GetAllAsync(string? searchQuery = null);
    Task<ContactDto?> GetByIdAsync(int id);
    Task<ContactDto?> CreateAsync(CreateContactRequest request);
    Task<ContactDto?> UpdateAsync(int id, UpdateContactRequest request);
    Task<bool> DeleteAsync(int id);
}
