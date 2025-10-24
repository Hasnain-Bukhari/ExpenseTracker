-- Drop transactions table if it exists
DROP TABLE IF EXISTS transactions CASCADE;

-- Create transactions table
CREATE TABLE transactions (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    account_id UUID NOT NULL,
    category_id UUID NOT NULL,
    sub_category_id UUID NOT NULL,
    description VARCHAR(500),
    amount DECIMAL(18,2) NOT NULL,
    transaction_date TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX idx_transactions_user_id ON transactions(user_id);
CREATE INDEX idx_transactions_account_id ON transactions(account_id);
CREATE INDEX idx_transactions_category_id ON transactions(category_id);
CREATE INDEX idx_transactions_sub_category_id ON transactions(sub_category_id);
CREATE INDEX idx_transactions_transaction_date ON transactions(transaction_date);
CREATE INDEX idx_transactions_created_at ON transactions(created_at);

-- Create composite indexes for common queries
CREATE INDEX idx_transactions_user_date ON transactions(user_id, transaction_date);
CREATE INDEX idx_transactions_user_account ON transactions(user_id, account_id);
CREATE INDEX idx_transactions_user_category ON transactions(user_id, category_id);

-- Add foreign key constraints (assuming the referenced tables exist)
-- Uncomment these if you want to enforce referential integrity
-- ALTER TABLE transactions ADD CONSTRAINT fk_transactions_user_id 
--     FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
-- 
-- ALTER TABLE transactions ADD CONSTRAINT fk_transactions_account_id 
--     FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE;
-- 
-- ALTER TABLE transactions ADD CONSTRAINT fk_transactions_category_id 
--     FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE;
-- 
-- ALTER TABLE transactions ADD CONSTRAINT fk_transactions_sub_category_id 
--     FOREIGN KEY (sub_category_id) REFERENCES sub_categories(id) ON DELETE SET NULL;

-- Add check constraints
ALTER TABLE transactions ADD CONSTRAINT chk_transactions_amount_positive 
    CHECK (amount > 0);

-- Description can be null or empty, so no constraint needed

-- Create trigger to automatically update updated_at timestamp
CREATE OR REPLACE FUNCTION update_transactions_updated_at()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_transactions_updated_at
    BEFORE UPDATE ON transactions
    FOR EACH ROW
    EXECUTE FUNCTION update_transactions_updated_at();

-- Add comments for documentation
COMMENT ON TABLE transactions IS 'Stores all financial transactions for users';
COMMENT ON COLUMN transactions.id IS 'Unique identifier for the transaction';
COMMENT ON COLUMN transactions.user_id IS 'ID of the user who owns this transaction';
COMMENT ON COLUMN transactions.account_id IS 'ID of the account from/to which the transaction occurred';
COMMENT ON COLUMN transactions.category_id IS 'ID of the category this transaction belongs to';
COMMENT ON COLUMN transactions.sub_category_id IS 'ID of the subcategory (optional)';
COMMENT ON COLUMN transactions.description IS 'Description of the transaction';
COMMENT ON COLUMN transactions.amount IS 'Transaction amount (always positive)';
COMMENT ON COLUMN transactions.transaction_date IS 'Date when the transaction occurred';
COMMENT ON COLUMN transactions.created_at IS 'Timestamp when the record was created';
COMMENT ON COLUMN transactions.updated_at IS 'Timestamp when the record was last updated';
