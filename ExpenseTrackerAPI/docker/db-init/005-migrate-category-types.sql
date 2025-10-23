-- Migration script to convert Category type from enum to entity
-- Run this script before starting the application

-- Step 1: Create category_types table
CREATE TABLE IF NOT EXISTS category_types (
    id UUID PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    color VARCHAR(20),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Step 2: Insert default category types
INSERT INTO category_types (id, name, description, color, is_active, created_at, updated_at) VALUES
    ('11111111-1111-1111-1111-111111111111', 'Expense', 'Categories for tracking expenses', '#f44336', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('22222222-2222-2222-2222-222222222222', 'Income', 'Categories for tracking income', '#4caf50', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('33333333-3333-3333-3333-333333333333', 'Investment', 'Categories for tracking investments', '#2196f3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
ON CONFLICT (id) DO NOTHING;

-- Step 3: Add category_type_id column to categories table
ALTER TABLE categories ADD COLUMN IF NOT EXISTS category_type_id UUID;

-- Step 4: Update existing categories to use the new category_type_id
-- Map existing enum values to the new category types
UPDATE categories 
SET category_type_id = CASE 
    WHEN type = 'Expense' THEN '11111111-1111-1111-1111-111111111111'
    WHEN type = 'Income' THEN '22222222-2222-2222-2222-222222222222'
    WHEN type = 'Investment' THEN '33333333-3333-3333-3333-333333333333'
    ELSE '11111111-1111-1111-1111-111111111111' -- Default to Expense
END
WHERE category_type_id IS NULL;

-- Step 5: Make category_type_id NOT NULL after updating existing records
ALTER TABLE categories ALTER COLUMN category_type_id SET NOT NULL;

-- Step 6: Add foreign key constraint
ALTER TABLE categories 
ADD CONSTRAINT fk_categories_category_type 
FOREIGN KEY (category_type_id) REFERENCES category_types(id);

-- Step 7: Create index for better performance
CREATE INDEX IF NOT EXISTS idx_categories_category_type_id ON categories(category_type_id);

-- Step 8: Drop the old type column (optional - comment out if you want to keep it for rollback)
-- ALTER TABLE categories DROP COLUMN IF EXISTS type;

-- Step 9: Add parent_id column if it doesn't exist (for future hierarchical categories)
ALTER TABLE categories ADD COLUMN IF NOT EXISTS parent_id UUID;

-- Add foreign key for parent_id (self-reference)
ALTER TABLE categories 
ADD CONSTRAINT fk_categories_parent 
FOREIGN KEY (parent_id) REFERENCES categories(id);

-- Create index for parent_id
CREATE INDEX IF NOT EXISTS idx_categories_parent_id ON categories(parent_id);

-- Verification queries (run these to check the migration)
-- SELECT 'Category Types:' as info;
-- SELECT id, name, description, color, is_active FROM category_types ORDER BY name;
-- 
-- SELECT 'Categories with Types:' as info;
-- SELECT c.id, c.name, c.description, ct.name as type_name, ct.color 
-- FROM categories c 
-- JOIN category_types ct ON c.category_type_id = ct.id 
-- ORDER BY c.name;
