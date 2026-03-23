using System.Security.Claims;
using ContactManager.Api.Services.Interfaces;
using ContactManager.Common.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Api.Controllers;

[ApiController]
[Route("api/contacts")]
[Authorize]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")
        ?? throw new UnauthorizedAccessException("User ID not found in token.");

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContactDto>>>> GetAll([FromQuery] string? q)
    {
        var contacts = string.IsNullOrWhiteSpace(q)
            ? await _contactService.GetAllAsync(UserId)
            : await _contactService.SearchAsync(UserId, q);

        return Ok(ApiResponse<IEnumerable<ContactDto>>.Ok(contacts));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> GetById(int id)
    {
        var contact = await _contactService.GetByIdAsync(id, UserId);
        if (contact is null)
            return NotFound(ApiResponse<ContactDto>.Fail("Contact not found."));

        return Ok(ApiResponse<ContactDto>.Ok(contact));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Create([FromBody] CreateContactRequest request)
    {
        var contact = await _contactService.CreateAsync(request, UserId);
        return CreatedAtAction(nameof(GetById), new { id = contact.Id },
            ApiResponse<ContactDto>.Ok(contact));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Update(int id, [FromBody] UpdateContactRequest request)
    {
        var contact = await _contactService.UpdateAsync(id, request, UserId);
        if (contact is null)
            return NotFound(ApiResponse<ContactDto>.Fail("Contact not found."));

        return Ok(ApiResponse<ContactDto>.Ok(contact));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var deleted = await _contactService.DeleteAsync(id, UserId);
        if (!deleted)
            return NotFound(ApiResponse<bool>.Fail("Contact not found."));

        return Ok(ApiResponse<bool>.Ok(true));
    }
}
