-- Add unique constraint on (user_id, code) combination for currencies
-- This ensures currency code is unique per user, not globally unique
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
