-- Create category_types table
CREATE TABLE IF NOT EXISTS category_types (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    color VARCHAR(20),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Insert default category types
INSERT INTO category_types (id, name, description, color, is_active, created_at, updated_at) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Expense', 'Categories for tracking expenses', '#f44336', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('22222222-2222-2222-2222-222222222222', 'Income', 'Categories for tracking income', '#4caf50', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('33333333-3333-3333-3333-333333333333', 'Investment', 'Categories for tracking investments', '#2196f3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;
