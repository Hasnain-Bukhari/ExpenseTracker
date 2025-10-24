-- Basic table creation script for ExpenseTracker
-- This script creates all the necessary tables

-- Create users table first (referenced by other tables)
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY,
    email VARCHAR(255) NOT NULL UNIQUE,
    normalized_email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255),
    full_name VARCHAR(255) NOT NULL,
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

-- Create currencies table
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

-- Create account_types table
CREATE TABLE IF NOT EXISTS account_types (
    id UUID PRIMARY KEY,
    user_id UUID,
    name VARCHAR(100) NOT NULL,
    is_card BOOLEAN NOT NULL DEFAULT false,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_account_types_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create accounts table
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

-- Create categories table
CREATE TABLE IF NOT EXISTS categories (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    category_type VARCHAR(50) NOT NULL CHECK (category_type IN ('Income', 'Expense')),
    parent_id UUID,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_categories_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_categories_parent_id FOREIGN KEY (parent_id) REFERENCES categories(id)
);

-- Create transactions table
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
    CONSTRAINT fk_transactions_sub_category_id FOREIGN KEY (sub_category_id) REFERENCES categories(id)
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_currencies_user_id ON currencies(user_id);
CREATE INDEX IF NOT EXISTS idx_account_types_user_id ON account_types(user_id);
CREATE INDEX IF NOT EXISTS idx_accounts_user_id ON accounts(user_id);
CREATE INDEX IF NOT EXISTS idx_categories_user_id ON categories(user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_user_id ON transactions(user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_account_id ON transactions(account_id);
CREATE INDEX IF NOT EXISTS idx_transactions_category_id ON transactions(category_id);
CREATE INDEX IF NOT EXISTS idx_transactions_date ON transactions(transaction_date);
