-- Contact Manager Database Initialization Script
-- Run this script against a PostgreSQL 14+ database

CREATE SCHEMA IF NOT EXISTS contacts;

-- Users table
CREATE TABLE IF NOT EXISTS contacts.users (
    "Id"           VARCHAR(450)  PRIMARY KEY,
    "Email"        VARCHAR(200)  NOT NULL UNIQUE,
    "PasswordHash" TEXT          NOT NULL,
    "CreatedAt"    TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

-- Contacts table
CREATE TABLE IF NOT EXISTS contacts.contacts (
    "Id"        SERIAL        PRIMARY KEY,
    "Name"      VARCHAR(200)  NOT NULL,
    "Phone"     VARCHAR(50),
    "Email"     VARCHAR(200),
    "Notes"     VARCHAR(1000),
    "UserId"    VARCHAR(450)  NOT NULL REFERENCES contacts.users("Id") ON DELETE CASCADE,
    "CreatedAt" TIMESTAMPTZ   NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_contacts_user_id ON contacts.contacts("UserId");
CREATE INDEX IF NOT EXISTS idx_contacts_name    ON contacts.contacts("Name");
CREATE INDEX IF NOT EXISTS idx_contacts_email   ON contacts.contacts("Email");

-- =============================================================================
-- Seed data
-- Demo user: admin@demo.com / Admin1234!
-- Password hash generated with BCrypt.Net-Next 4.0.3, work factor 11
-- =============================================================================
INSERT INTO contacts.users ("Id", "Email", "PasswordHash", "CreatedAt")
VALUES (
    'a1b2c3d4-0000-0000-0000-000000000001',
    'admin@demo.com',
    '$2a$11$b0v7jn7UCmei3gI/tqG.7eiFgBSvgZhz0MUVSoCNHT1Cxk7FVVEL6',
    NOW()
)
ON CONFLICT ("Id") DO NOTHING;

INSERT INTO contacts.contacts ("Name", "Phone", "Email", "Notes", "UserId", "CreatedAt", "UpdatedAt")
VALUES
    ('Anna Kowalska',    '+48 600 100 200', 'anna.kowalska@example.com', 'Klientka VIP',        'a1b2c3d4-0000-0000-0000-000000000001', NOW(), NOW()),
    ('Piotr Nowak',      '+48 601 200 300', 'piotr.nowak@example.com',   'Dostawca',            'a1b2c3d4-0000-0000-0000-000000000001', NOW(), NOW()),
    ('Maria Wiśniewska', '+48 602 300 400', 'maria.wisn@example.com',    'Partnerka biznesowa', 'a1b2c3d4-0000-0000-0000-000000000001', NOW(), NOW()),
    ('Jan Zieliński',    '+48 603 400 500', 'jan.zielinski@example.com', NULL,                  'a1b2c3d4-0000-0000-0000-000000000001', NOW(), NOW()),
    ('Katarzyna Wójcik', '+48 604 500 600', 'k.wojcik@example.com',      'Znajoma ze studiów',  'a1b2c3d4-0000-0000-0000-000000000001', NOW(), NOW())
ON CONFLICT DO NOTHING;
