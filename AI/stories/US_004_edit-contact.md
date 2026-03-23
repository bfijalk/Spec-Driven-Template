# US_004: Edit Contact

## User Story
**As a** logged-in user,
**I want** to edit existing contact information,
**So that** I can keep my contact details up-to-date and accurate.

## Acceptance Criteria
- [ ] User can access edit functionality from the contact list
- [ ] Edit form pre-populates with existing contact information
- [ ] User can modify any contact field (name, phone, email, notes)
- [ ] Form validates required fields and email format
- [ ] User can save changes and return to dashboard
- [ ] Updated contact information appears immediately in the contact list
- [ ] User can cancel editing and return to dashboard without saving changes

## Subtasks

### ST_001: Design Edit Contact Interface
**Type:** `Design`
**Estimate:** `S`
**Description:**
Create design for edit contact form and edit action triggers.

**Definition of Done:**
- [ ] Edit button/icon design for contact list items
- [ ] Edit form design (similar to add contact form)
- [ ] Pre-populated form state design
- [ ] Save and cancel button designs for edit mode

---

### ST_002: Implement Get Single Contact API
**Type:** `API`
**Estimate:** `S`
**Description:**
Create endpoint to retrieve individual contact details for editing.

**Definition of Done:**
- [ ] GET /api/contacts/:id endpoint created
- [ ] Contact ownership verification for authenticated user
- [ ] Proper error handling for non-existent contacts
- [ ] Contact data returned in appropriate format

---

### ST_003: Implement Update Contact API
**Type:** `API`
**Estimate:** `M`
**Description:**
Create endpoint to update existing contact information.

**Definition of Done:**
- [ ] PUT /api/contacts/:id endpoint created
- [ ] Request validation for required fields and email format
- [ ] Contact ownership verification before update
- [ ] Proper HTTP status codes and error responses

---

### ST_004: Add Edit Triggers to Contact List
**Type:** `Frontend`
**Estimate:** `S`
**Description:**
Add edit buttons or actions to each contact in the dashboard list.

**Definition of Done:**
- [ ] Edit button/icon added to each contact list item
- [ ] Click handler to navigate to edit form with contact ID
- [ ] Consistent styling with overall design
- [ ] Accessible button with proper labels

---

### ST_005: Build Edit Contact Form
**Type:** `Frontend`
**Estimate:** `L`
**Description:**
Implement edit form that loads existing contact data and handles updates.

**Definition of Done:**
- [ ] Edit form component with pre-populated fields
- [ ] API integration to fetch existing contact data
- [ ] Form submission handling for updates
- [ ] Loading state while fetching contact data

---

### ST_006: Implement Update Validation
**Type:** `Frontend`
**Estimate:** `S`
**Description:**
Add validation for edit form similar to add contact form.

**Definition of Done:**
- [ ] Client-side validation for required fields
- [ ] Email format validation with error messages
- [ ] Form submission disabled until validation passes
- [ ] Validation state preserved during editing

---

### ST_007: Add Update Database Operations
**Type:** `Database`
**Estimate:** `S`
**Description:**
Implement database operations for updating existing contact records.

**Definition of Done:**
- [ ] Database update operation for contact records
- [ ] Proper error handling for update constraints
- [ ] Transaction handling for data consistency
- [ ] Optimistic locking to prevent concurrent update conflicts

---

### ST_008: Test Edit Contact Functionality
**Type:** `Testing`
**Estimate:** `M`
**Description:**
Create comprehensive tests for editing existing contacts.

**Definition of Done:**
- [ ] Unit tests for get and update contact API endpoints
- [ ] Frontend tests for edit form functionality
- [ ] Integration tests for complete edit contact flow
- [ ] Tests for error scenarios and validation

## Notes
- Consider showing unsaved changes warning if user navigates away
- Ensure edit form maintains same validation rules as add contact form
- Plan for handling concurrent edits by multiple sessions (if applicable)