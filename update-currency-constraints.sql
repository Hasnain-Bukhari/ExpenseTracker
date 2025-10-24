-- Update currency constraints to be user-specific
-- First, drop the existing unique constraint and index on code if they exist
DO $$
BEGIN
    -- Drop existing unique constraint on code column
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'uk_currencies_code') THEN
        ALTER TABLE currencies DROP CONSTRAINT uk_currencies_code;
    END IF;
    
    -- Drop any other unique constraints on code that might exist
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'currencies_code_key') THEN
        ALTER TABLE currencies DROP CONSTRAINT currencies_code_key;
    END IF;
    
    -- Drop any other unique constraints on code that might exist
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'currencies_unique') THEN
        ALTER TABLE currencies DROP CONSTRAINT currencies_unique;
    END IF;
END
$$;

-- Drop the old unique index on code column
DROP INDEX IF EXISTS ux_currencies_code;

-- Add new unique constraint on (user_id, code) combination
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'uk_currencies_user_code') THEN
        ALTER TABLE currencies
        ADD CONSTRAINT uk_currencies_user_code
        UNIQUE (user_id, code);
    END IF;
END
$$;

-- Add comments
COMMENT ON CONSTRAINT uk_currencies_user_code ON currencies IS 'Ensures currency code is unique per user.';
