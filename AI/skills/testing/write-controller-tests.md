# Skill: Write Controller Tests

## When to Use
When writing xUnit tests for API controllers in `ContactManager.Tests/`.

## System Prompt

You are writing controller-level unit tests for the **Contact Manager** API. Controller tests verify HTTP response codes, model binding, and authorization, using Moq to mock the service layer.

### Controller Test Pattern:
```csharp
using System.Security.Claims;
using ContactManager.Api.Controllers;
using ContactManager.Api.Services.Interfaces;
using ContactManager.Common.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContactManager.Tests.Controllers;

public class ContactsControllerTests
{
    private readonly Mock<IContactService> _serviceMock;
    private readonly ContactsController _controller;
    private const string TestUserId = "user-123";

    public ContactsControllerTests()
    {
        _serviceMock = new Mock<IContactService>();
        _controller = new ContactsController(_serviceMock.Object);
        
        // Set up fake authenticated user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, TestUserId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithContacts()
    {
        // Arrange
        var contacts = new List<ContactDto>
        {
            new() { Id = 1, Name = "Alice", UserId = TestUserId }
        };
        _serviceMock.Setup(s => s.GetAllAsync(TestUserId))
            .ReturnsAsync(contacts);

        // Act
        var result = await _controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<ContactDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.Setup(s => s.GetByIdAsync(999, TestUserId))
            .ReturnsAsync((ContactDto?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateContactRequest { Name = "Alice" };
        var created = new ContactDto { Id = 1, Name = "Alice", UserId = TestUserId };
        _serviceMock.Setup(s => s.CreateAsync(request, TestUserId))
            .ReturnsAsync(created);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
    }
}
```

### Rules:
1. **Mock services with Moq** — `new Mock<IXxxService>()` — do NOT use real implementations
2. **Set up fake user** — `ClaimsPrincipal` with `ClaimTypes.NameIdentifier` in `ControllerContext`
3. **Test HTTP responses:** `OkObjectResult`, `NotFoundObjectResult`, `CreatedAtActionResult`, `BadRequestObjectResult`
4. **Verify `ApiResponse<T>`** — check `.Success` and `.Data` on the response wrapper
5. **Test naming:** `ActionName_Scenario_ExpectedHttpResult`
6. **Verify service calls:** `_serviceMock.Verify(s => s.Method(...), Times.Once)`
7. **One test class per controller** — `XxxControllerTests.cs`
8. **Standard scenarios:**
   - GET all → 200 OK
   - GET by ID found → 200 OK
   - GET by ID not found → 404
   - POST valid → 201 Created
   - PUT valid → 200 OK
   - PUT not found → 404
   - DELETE valid → 200 OK
   - DELETE not found → 404

## Input Expected
- Controller name and its action methods
- Service interface with method signatures
- Expected HTTP responses per scenario

## Output
- Complete test class with constructor setup and all test methods
- File path: `ContactManager.Tests/Controllers/XxxControllerTests.cs`
