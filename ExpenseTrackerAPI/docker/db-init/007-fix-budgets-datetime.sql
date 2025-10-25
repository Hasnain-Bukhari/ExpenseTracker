-- Fix budgets table to use TIMESTAMP instead of TIMESTAMP WITH TIME ZONE
-- This migration updates the existing budgets table to match DateTime in C#

-- First, drop the existing table if it exists with wrong column types
DROP TABLE IF EXISTS budgets CASCADE;

-- Recreate the budgets table with correct TIMESTAMP columns
CREATE TABLE budgets (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    category_id UUID NOT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    effective_from DATE NOT NULL,
    effective_to DATE NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign key constraints
    CONSTRAINT fk_budgets_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_budgets_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE,
    
    -- Ensure only one active budget per category per user
    CONSTRAINT uk_budgets_user_category_active UNIQUE (user_id, category_id) DEFERRABLE INITIALLY DEFERRED
);

-- Create indexes for performance
CREATE INDEX idx_budgets_user_category_active ON budgets(user_id, category_id, is_active);
CREATE INDEX idx_budgets_effective_dates ON budgets(effective_from, effective_to);
CREATE INDEX idx_budgets_user_active ON budgets(user_id, is_active);

-- Add comments
COMMENT ON TABLE budgets IS 'Time-based budget system with effective date ranges';
COMMENT ON COLUMN budgets.effective_from IS 'Start date of budget effectiveness (start of month)';
COMMENT ON COLUMN budgets.effective_to IS 'End date of budget effectiveness (NULL for active budgets)';
COMMENT ON COLUMN budgets.is_active IS 'Whether this budget is currently active';
