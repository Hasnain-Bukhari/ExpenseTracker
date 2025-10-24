-- Fix transactions table schema to match entity
-- This script updates the existing transactions table

-- Drop the problematic constraint
ALTER TABLE transactions DROP CONSTRAINT IF EXISTS chk_transactions_description_not_empty;

-- Update sub_category_id to be NOT NULL (if it's currently nullable)
-- First, update any NULL values to a default subcategory if they exist
-- UPDATE transactions SET sub_category_id = '00000000-0000-0000-0000-000000000000' WHERE sub_category_id IS NULL;

-- Make sub_category_id NOT NULL
ALTER TABLE transactions ALTER COLUMN sub_category_id SET NOT NULL;

-- Make description nullable (if it's currently NOT NULL)
ALTER TABLE transactions ALTER COLUMN description DROP NOT NULL;

-- Add comment
COMMENT ON COLUMN transactions.sub_category_id IS 'ID of the subcategory (required)';
COMMENT ON COLUMN transactions.description IS 'Description of the transaction (optional)';
