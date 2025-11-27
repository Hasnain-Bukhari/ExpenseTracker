-- Add preferred_name column to users table
-- This script adds a new column for user's preferred name

ALTER TABLE users 
ADD COLUMN IF NOT EXISTS preferred_name VARCHAR(255);

-- Add comment to the column
COMMENT ON COLUMN users.preferred_name IS 'User preferred display name for personalization';

