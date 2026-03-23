using ContactManager.Common.DTOs;

namespace ContactManager.Api.Services.Interfaces;

public interface IContactService
{
    Task<IEnumerable<ContactDto>> GetAllAsync(string userId);
    Task<IEnumerable<ContactDto>> SearchAsync(string userId, string query);
    Task<ContactDto?> GetByIdAsync(int id, string userId);
    Task<ContactDto> CreateAsync(CreateContactRequest request, string userId);
    Task<ContactDto?> UpdateAsync(int id, UpdateContactRequest request, string userId);
    Task<bool> DeleteAsync(int id, string userId);
}
