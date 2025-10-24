-- Migration script to add profile_image and default_account_id to users table
-- Run this script to update the existing users table

-- Add profile_image column
ALTER TABLE users ADD COLUMN IF NOT EXISTS profile_image VARCHAR(500);

-- Add default_account_id column
ALTER TABLE users ADD COLUMN IF NOT EXISTS default_account_id UUID;

-- Add foreign key constraint for default_account_id
ALTER TABLE users 
ADD CONSTRAINT fk_users_default_account 
FOREIGN KEY (default_account_id) 
REFERENCES accounts(id) 
ON DELETE SET NULL;

-- Add comments for better documentation
COMMENT ON COLUMN users.profile_image IS 'URL or path to the user profile image';
COMMENT ON COLUMN users.default_account_id IS 'ID of the default account for the user';

-- Create index for performance
CREATE INDEX IF NOT EXISTS idx_users_default_account_id ON users (default_account_id);
