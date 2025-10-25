-- Migration script to update goals table structure
-- This script removes old columns and adds new ones for the updated Goal entity

-- First, drop the old goals table if it exists
DROP TABLE IF EXISTS goals CASCADE;

-- Create the new goals table with updated structure
CREATE TABLE goals (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    target_amount DECIMAL(18,2) NOT NULL,
    current_amount DECIMAL(18,2) NOT NULL DEFAULT 0,
    category_id UUID NOT NULL,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP NULL,
    tag VARCHAR(100),
    status VARCHAR(50) NOT NULL DEFAULT 'Active',
    priority VARCHAR(50) NOT NULL DEFAULT 'Medium',
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Foreign key constraints
    CONSTRAINT fk_goals_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_goals_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE,
    
    -- Check constraints
    CONSTRAINT chk_goals_target_amount CHECK (target_amount > 0),
    CONSTRAINT chk_goals_current_amount CHECK (current_amount >= 0),
    CONSTRAINT chk_goals_end_date CHECK (end_date IS NULL OR end_date >= start_date),
    CONSTRAINT chk_goals_status CHECK (status IN ('Active', 'Paused', 'Completed', 'Cancelled')),
    CONSTRAINT chk_goals_priority CHECK (priority IN ('Low', 'Medium', 'High'))
);

-- Create indexes for better performance
CREATE INDEX idx_goals_user_id ON goals(user_id);
CREATE INDEX idx_goals_category_id ON goals(category_id);
CREATE INDEX idx_goals_status ON goals(status);
CREATE INDEX idx_goals_priority ON goals(priority);
CREATE INDEX idx_goals_start_date ON goals(start_date);
CREATE INDEX idx_goals_end_date ON goals(end_date);

-- Add comments for documentation
COMMENT ON TABLE goals IS 'User financial goals with progress tracking';
COMMENT ON COLUMN goals.target_amount IS 'The target amount to achieve for this goal';
COMMENT ON COLUMN goals.current_amount IS 'Current progress amount towards the goal';
COMMENT ON COLUMN goals.category_id IS 'Category this goal is associated with (must be TargetedSavingsGoal type)';
COMMENT ON COLUMN goals.start_date IS 'When the goal was started';
COMMENT ON COLUMN goals.end_date IS 'Optional end date for the goal (NULL for open-ended goals)';
COMMENT ON COLUMN goals.tag IS 'Optional tag for categorizing goals';
COMMENT ON COLUMN goals.status IS 'Current status of the goal (Active, Paused, Completed, Cancelled)';
COMMENT ON COLUMN goals.priority IS 'Priority level of the goal (Low, Medium, High)';
