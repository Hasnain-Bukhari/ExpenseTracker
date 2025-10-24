-- Add user_id column to account_types table
ALTER TABLE account_types
ADD COLUMN IF NOT EXISTS user_id UUID;

-- Add foreign key constraint for user_id
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'fk_account_types_user_id') THEN
        ALTER TABLE account_types
        ADD CONSTRAINT fk_account_types_user_id
        FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
    END IF;
END
$$;

-- Add comments
COMMENT ON COLUMN account_types.user_id IS 'ID of the user who owns this account type.';

-- Update existing account types to be owned by a default user (if any exists)
-- This is a temporary fix - in production you'd want to handle this differently
UPDATE account_types 
SET user_id = (SELECT id FROM users LIMIT 1)
WHERE user_id IS NULL AND EXISTS (SELECT 1 FROM users);
