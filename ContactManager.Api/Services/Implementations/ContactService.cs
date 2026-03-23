using ContactManager.Api.Services.Interfaces;
using ContactManager.Common.DTOs;
using ContactManager.Database.Entities;
using ContactManager.Database.Repositories.Interfaces;

namespace ContactManager.Api.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;

    public ContactService(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<IEnumerable<ContactDto>> GetAllAsync(string userId)
    {
        var contacts = await _contactRepository.GetAllByUserIdAsync(userId);
        return contacts.Select(ToDto);
    }

    public async Task<IEnumerable<ContactDto>> SearchAsync(string userId, string query)
    {
        var contacts = await _contactRepository.SearchAsync(userId, query);
        return contacts.Select(ToDto);
    }

    public async Task<ContactDto?> GetByIdAsync(int id, string userId)
    {
        var contact = await _contactRepository.GetByIdAsync(id, userId);
        return contact is null ? null : ToDto(contact);
    }

    public async Task<ContactDto> CreateAsync(CreateContactRequest request, string userId)
    {
        var contact = new Contact
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            Notes = request.Notes,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _contactRepository.CreateAsync(contact);
        return ToDto(created);
    }

    public async Task<ContactDto?> UpdateAsync(int id, UpdateContactRequest request, string userId)
    {
        var contact = await _contactRepository.GetByIdAsync(id, userId);
        if (contact is null) return null;

        contact.Name = request.Name;
        contact.Phone = request.Phone;
        contact.Email = request.Email;
        contact.Notes = request.Notes;
        contact.UpdatedAt = DateTime.UtcNow;

        var updated = await _contactRepository.UpdateAsync(contact);
        return ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id, string userId)
    {
        var contact = await _contactRepository.GetByIdAsync(id, userId);
        if (contact is null) return false;

        await _contactRepository.DeleteAsync(contact);
        return true;
    }

    private static ContactDto ToDto(Contact c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Phone = c.Phone,
        Email = c.Email,
        Notes = c.Notes,
        UserId = c.UserId,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
