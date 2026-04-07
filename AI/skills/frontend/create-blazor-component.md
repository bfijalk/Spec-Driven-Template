# Skill: Create Blazor Component

## When to Use
When building a reusable component in `ContactManager/Components/Shared/`.

## System Prompt

You are building reusable Blazor components for the **Contact Manager** application. Components go in `Components/Shared/` and are used across multiple pages.

### Existing Component Patterns:

**ContactCard.razor** — Display component with events:
```razor
<div class="contact-card">
    <div class="contact-info">
        <div class="contact-name">@Contact.Name</div>
        @* details *@
    </div>
    <div class="contact-actions">
        <a href="/contacts/edit/@Contact.Id" class="btn btn-sm btn-outline-secondary">Edit</a>
        <button class="btn btn-sm btn-outline-danger" @onclick="() => OnDelete.InvokeAsync(Contact)">Delete</button>
    </div>
</div>

@code {
    [Parameter, EditorRequired] public ContactDto Contact { get; set; } = null!;
    [Parameter] public EventCallback<ContactDto> OnDelete { get; set; }
}
```

**EmptyState.razor** — Parametric UI component:
```razor
@code {
    [Parameter, EditorRequired] public string Title { get; set; } = null!;
    [Parameter] public string? Message { get; set; }
    [Parameter] public string? ActionLabel { get; set; }
    [Parameter] public EventCallback ActionCallback { get; set; }
}
```

**DeleteConfirmationDialog.razor** — Modal dialog:
```razor
@code {
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public string ContactName { get; set; } = string.Empty;
    [Parameter] public EventCallback OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
}
```

### Rules:
1. **Parameters:** Use `[Parameter]` for all inputs, `[EditorRequired]` for mandatory ones
2. **Events:** Use `EventCallback` / `EventCallback<T>` for parent communication
3. **No service injection** in display components — data comes from parameters
4. **Null safety:** Use `= null!` for required reference parameters, `= string.Empty` for strings
5. **Bootstrap 5:** Use Bootstrap classes, custom CSS goes in `wwwroot/app.css`
6. **Naming:** PascalCase component names, descriptive (e.g., `DeleteConfirmationDialog`, not `Modal`)
7. **Single Responsibility:** One component, one visual concern
8. **No page routes** — components don't have `@page` directive
9. **CSS isolation:** Prefer shared `app.css` over component-scoped CSS (project convention)
10. **SVG icons:** Inline SVG icons from Bootstrap Icons (following existing ContactCard pattern)

### Component Categories:
- **Display:** Shows data from parameters (ContactCard, EmptyState)
- **Input:** Captures user input, fires events (SearchInput, form fields)
- **Dialog:** Modal/overlay with confirm/cancel (DeleteConfirmationDialog)
- **Layout:** Structural wrappers (LoadingSpinner, PageHeader)

## Input Expected
- Component purpose / what it displays
- Parameters it needs (with types)
- Events it fires
- Where it will be used

## Output
- Complete `.razor` file with markup + `@code` block
- CSS additions for `wwwroot/app.css` (if needed)
- Usage example showing how to include in a page
