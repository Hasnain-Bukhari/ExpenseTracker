-- ============================================================================
-- Complete Database Schema Script for Expense Tracker Production Database
-- This script creates all tables, constraints, indexes, and relationships
-- Run this script on your production database (expensetrackerdb)
-- ============================================================================

-- ============================================================================
-- STEP 1: Create Core Tables (in dependency order)
-- ============================================================================

-- 1. Users table (base table, referenced by all others)
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    normalized_email VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255),
    full_name VARCHAR(255),
    default_currency_id UUID,
    locale VARCHAR(10) DEFAULT 'en-US',
    timezone VARCHAR(50) DEFAULT 'UTC',
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_email_verified BOOLEAN NOT NULL DEFAULT false,
    phone VARCHAR(20),
    profile_image TEXT,
    default_account_id UUID,
    provider VARCHAR(50) NOT NULL DEFAULT 'Local',
    provider_id VARCHAR(255),
    last_login_at TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Unique constraints on users
CREATE UNIQUE INDEX IF NOT EXISTS ux_users_email ON users(email);
CREATE UNIQUE INDEX IF NOT EXISTS ux_users_normalized_email ON users(normalized_email);

-- 2. Currencies table
CREATE TABLE IF NOT EXISTS currencies (
    id UUID PRIMARY KEY,
    user_id UUID,
    code VARCHAR(10) NOT NULL,
    symbol VARCHAR(10),
    name VARCHAR(100),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_currencies_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Unique constraint: currency code must be unique per user
CREATE UNIQUE INDEX IF NOT EXISTS uk_currencies_user_code ON currencies(user_id, code) WHERE user_id IS NOT NULL;

-- Index for performance
CREATE INDEX IF NOT EXISTS idx_currencies_user_id ON currencies(user_id);

-- 3. Account Types table
CREATE TABLE IF NOT EXISTS account_types (
    id UUID PRIMARY KEY,
    user_id UUID,
    name VARCHAR(100) NOT NULL,
    is_card BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_account_types_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Index for performance
CREATE INDEX IF NOT EXISTS idx_account_types_user_id ON account_types(user_id);

-- 4. Accounts table
CREATE TABLE IF NOT EXISTS accounts (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    account_type_id UUID NOT NULL,
    currency_id UUID NOT NULL,
    is_savings BOOLEAN NOT NULL DEFAULT false,
    opening_balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    include_in_networth BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_accounts_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_accounts_account_type_id FOREIGN KEY (account_type_id) REFERENCES account_types(id),
    CONSTRAINT fk_accounts_currency_id FOREIGN KEY (currency_id) REFERENCES currencies(id)
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_accounts_user_id ON accounts(user_id);
CREATE INDEX IF NOT EXISTS idx_accounts_account_type_id ON accounts(account_type_id);
CREATE INDEX IF NOT EXISTS idx_accounts_currency_id ON accounts(currency_id);

-- 5. Categories table
CREATE TABLE IF NOT EXISTS categories (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    parent_id UUID,
    category_type VARCHAR(50) NOT NULL CHECK (category_type IN ('Income', 'Expense', 'TargetedSavingsGoal')),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_categories_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_categories_parent_id FOREIGN KEY (parent_id) REFERENCES categories(id)
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_categories_user_id ON categories(user_id);
CREATE INDEX IF NOT EXISTS idx_categories_parent_id ON categories(parent_id);
CREATE INDEX IF NOT EXISTS idx_categories_category_type ON categories(category_type);

-- 6. Sub Categories table
CREATE TABLE IF NOT EXISTS sub_categories (
    id UUID PRIMARY KEY,
    category_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_sub_categories_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_sub_categories_category_id ON sub_categories(category_id);

-- 7. Transactions table
CREATE TABLE IF NOT EXISTS transactions (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    account_id UUID NOT NULL,
    category_id UUID NOT NULL,
    sub_category_id UUID,
    description TEXT,
    amount DECIMAL(18,2) NOT NULL,
    transaction_date DATE NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_transactions_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_transactions_account_id FOREIGN KEY (account_id) REFERENCES accounts(id),
    CONSTRAINT fk_transactions_category_id FOREIGN KEY (category_id) REFERENCES categories(id),
    CONSTRAINT fk_transactions_sub_category_id FOREIGN KEY (sub_category_id) REFERENCES sub_categories(id)
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_transactions_user_id ON transactions(user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_account_id ON transactions(account_id);
CREATE INDEX IF NOT EXISTS idx_transactions_category_id ON transactions(category_id);
CREATE INDEX IF NOT EXISTS idx_transactions_sub_category_id ON transactions(sub_category_id);
CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(transaction_date);
CREATE INDEX IF NOT EXISTS idx_transactions_user_date ON transactions(user_id, transaction_date);

-- 8. Budgets table
CREATE TABLE IF NOT EXISTS budgets (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    category_id UUID NOT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    effective_from DATE NOT NULL,
    effective_to DATE,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_budgets_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_budgets_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE
);

-- Unique constraint: only one active budget per category per user
CREATE UNIQUE INDEX IF NOT EXISTS uk_budgets_user_category_active ON budgets(user_id, category_id) 
WHERE is_active = true;

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_budgets_user_category_active ON budgets(user_id, category_id, is_active);
CREATE INDEX IF NOT EXISTS idx_budgets_effective_dates ON budgets(effective_from, effective_to);
CREATE INDEX IF NOT EXISTS idx_budgets_user_active ON budgets(user_id, is_active);

-- 9. Goals table
CREATE TABLE IF NOT EXISTS goals (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    target_amount DECIMAL(18,2) NOT NULL,
    current_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    category_id UUID NOT NULL,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP,
    tag VARCHAR(100),
    status VARCHAR(50) NOT NULL DEFAULT 'Active',
    priority VARCHAR(50) NOT NULL DEFAULT 'Medium',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_goals_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_goals_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE,
    CONSTRAINT chk_goals_target_amount CHECK (target_amount > 0),
    CONSTRAINT chk_goals_current_amount CHECK (current_amount >= 0),
    CONSTRAINT chk_goals_end_date CHECK (end_date IS NULL OR end_date >= start_date),
    CONSTRAINT chk_goals_status CHECK (status IN ('Active', 'Paused', 'Completed', 'Cancelled')),
    CONSTRAINT chk_goals_priority CHECK (priority IN ('Low', 'Medium', 'High'))
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_goals_user_id ON goals(user_id);
CREATE INDEX IF NOT EXISTS idx_goals_category_id ON goals(category_id);
CREATE INDEX IF NOT EXISTS idx_goals_status ON goals(status);
CREATE INDEX IF NOT EXISTS idx_goals_priority ON goals(priority);
CREATE INDEX IF NOT EXISTS idx_goals_start_date ON goals(start_date);
CREATE INDEX IF NOT EXISTS idx_goals_end_date ON goals(end_date);

-- 10. Refresh Tokens table
CREATE TABLE IF NOT EXISTS refresh_tokens (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    token_hash VARCHAR(255) NOT NULL,
    device_info VARCHAR(255),
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    revoked_at TIMESTAMP WITH TIME ZONE,
    CONSTRAINT fk_refresh_tokens_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token_hash ON refresh_tokens(token_hash);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires_at ON refresh_tokens(expires_at);

-- 11. Password Reset Tokens table
CREATE TABLE IF NOT EXISTS password_reset_tokens (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    token_hash VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP NOT NULL,
    used BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_password_reset_tokens_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Indexes for performance
CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_user_id ON password_reset_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_token_hash ON password_reset_tokens(token_hash);
CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_expires_at ON password_reset_tokens(expires_at);

-- ============================================================================
-- STEP 2: Add Foreign Key Constraints for User References
-- ============================================================================

-- Add foreign key for users.default_currency_id (can be NULL)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'fk_users_default_currency_id'
    ) THEN
        ALTER TABLE users
        ADD CONSTRAINT fk_users_default_currency_id 
        FOREIGN KEY (default_currency_id) REFERENCES currencies(id);
    END IF;
END $$;

-- Add foreign key for users.default_account_id (can be NULL)
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint 
        WHERE conname = 'fk_users_default_account_id'
    ) THEN
        ALTER TABLE users
        ADD CONSTRAINT fk_users_default_account_id 
        FOREIGN KEY (default_account_id) REFERENCES accounts(id);
    END IF;
END $$;

-- ============================================================================
-- STEP 3: Add Table Comments for Documentation
-- ============================================================================

COMMENT ON TABLE users IS 'User accounts and authentication information';
COMMENT ON TABLE currencies IS 'Currency definitions (unique code per user)';
COMMENT ON TABLE account_types IS 'Types of accounts (e.g., Checking, Savings, Credit Card)';
COMMENT ON TABLE accounts IS 'User financial accounts';
COMMENT ON TABLE categories IS 'Transaction categories (Income, Expense, TargetedSavingsGoal)';
COMMENT ON TABLE sub_categories IS 'Sub-categories within parent categories';
COMMENT ON TABLE transactions IS 'Financial transactions';
COMMENT ON TABLE budgets IS 'Time-based budget system with effective date ranges';
COMMENT ON TABLE goals IS 'User financial goals with progress tracking';
COMMENT ON TABLE refresh_tokens IS 'JWT refresh tokens for user sessions';
COMMENT ON TABLE password_reset_tokens IS 'Password reset tokens for forgot password flow';

-- ============================================================================
-- Script Complete!
-- ============================================================================
-- All tables, constraints, indexes, and relationships have been created.
-- You can now run your application against this database schema.
-- ============================================================================

