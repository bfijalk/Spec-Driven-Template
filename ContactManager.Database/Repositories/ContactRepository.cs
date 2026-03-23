using ContactManager.Database.Data;
using ContactManager.Database.Entities;
using ContactManager.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Database.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contact>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Contacts
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contact>> SearchAsync(string userId, string query)
    {
        var lower = query.ToLower();
        return await _context.Contacts
            .Where(c => c.UserId == userId &&
                (c.Name.ToLower().Contains(lower) ||
                 (c.Email != null && c.Email.ToLower().Contains(lower)) ||
                 (c.Phone != null && c.Phone.ToLower().Contains(lower))))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(int id, string userId)
    {
        return await _context.Contacts
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<Contact> UpdateAsync(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task DeleteAsync(Contact contact)
    {
        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
    }
}
