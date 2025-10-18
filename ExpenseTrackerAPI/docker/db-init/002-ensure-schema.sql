-- Safe idempotent DDL to create missing tables/columns/constraints used by ExpenseTracker API.
-- Uses CREATE TABLE IF NOT EXISTS and ALTER TABLE ... ADD COLUMN IF NOT EXISTS to avoid errors when columns already exist.

-- USERS
CREATE TABLE IF NOT EXISTS users (
  id uuid PRIMARY KEY,
  email varchar(320) NOT NULL,
  normalized_email varchar(320) NOT NULL,
  password_hash text,
  full_name varchar(255),
  default_currency char(3) NOT NULL DEFAULT 'USD',
  locale varchar(10) DEFAULT 'en-US',
  timezone varchar(50) DEFAULT 'UTC',
  is_active boolean NOT NULL DEFAULT true,
  is_email_verified boolean NOT NULL DEFAULT false,
  phone varchar(25),
  provider varchar(20) DEFAULT 'local',
  provider_id varchar(255),
  last_login_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

-- Ensure required columns exist (safe for schema drift)
ALTER TABLE users ADD COLUMN IF NOT EXISTS email varchar(320) NOT NULL;
ALTER TABLE users ADD COLUMN IF NOT EXISTS normalized_email varchar(320) NOT NULL;
ALTER TABLE users ADD COLUMN IF NOT EXISTS password_hash text;
ALTER TABLE users ADD COLUMN IF NOT EXISTS full_name varchar(255);
ALTER TABLE users ADD COLUMN IF NOT EXISTS default_currency char(3) NOT NULL DEFAULT 'USD';
ALTER TABLE users ADD COLUMN IF NOT EXISTS locale varchar(10) DEFAULT 'en-US';
ALTER TABLE users ADD COLUMN IF NOT EXISTS timezone varchar(50) DEFAULT 'UTC';
ALTER TABLE users ADD COLUMN IF NOT EXISTS is_active boolean NOT NULL DEFAULT true;
ALTER TABLE users ADD COLUMN IF NOT EXISTS is_email_verified boolean NOT NULL DEFAULT false;
ALTER TABLE users ADD COLUMN IF NOT EXISTS phone varchar(25);
ALTER TABLE users ADD COLUMN IF NOT EXISTS provider varchar(20) DEFAULT 'local';
ALTER TABLE users ADD COLUMN IF NOT EXISTS provider_id varchar(255);
ALTER TABLE users ADD COLUMN IF NOT EXISTS last_login_at timestamptz;
ALTER TABLE users ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE users ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

-- Unique index on email and index on normalized_email
CREATE UNIQUE INDEX IF NOT EXISTS ux_users_email ON users(email);
CREATE INDEX IF NOT EXISTS idx_users_normalized_email ON users(normalized_email);

-- REFRESH TOKENS
CREATE TABLE IF NOT EXISTS refresh_tokens (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  token_hash text NOT NULL,
  device_info text,
  expires_at timestamptz NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  revoked_at timestamptz
);

ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS token_hash text NOT NULL;
ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS device_info text;
ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS expires_at timestamptz NOT NULL;
ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS revoked_at timestamptz;

-- Add FK if missing
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_refresh_tokens_user_id'
  ) THEN
    ALTER TABLE refresh_tokens ADD CONSTRAINT fk_refresh_tokens_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);

-- PASSWORD RESET TOKENS
CREATE TABLE IF NOT EXISTS password_reset_tokens (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  token_hash text NOT NULL,
  expires_at timestamptz NOT NULL,
  used boolean NOT NULL DEFAULT false,
  created_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE password_reset_tokens ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE password_reset_tokens ADD COLUMN IF NOT EXISTS token_hash text NOT NULL;
ALTER TABLE password_reset_tokens ADD COLUMN IF NOT EXISTS expires_at timestamptz NOT NULL;
ALTER TABLE password_reset_tokens ADD COLUMN IF NOT EXISTS used boolean NOT NULL DEFAULT false;
ALTER TABLE password_reset_tokens ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_password_reset_tokens_user_id'
  ) THEN
    ALTER TABLE password_reset_tokens ADD CONSTRAINT fk_password_reset_tokens_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_user_id ON password_reset_tokens(user_id);

-- CURRENCIES (optional)
CREATE TABLE IF NOT EXISTS currencies (
  code char(3) PRIMARY KEY,
  symbol text,
  name text
);
ALTER TABLE currencies ADD COLUMN IF NOT EXISTS code char(3);
ALTER TABLE currencies ADD COLUMN IF NOT EXISTS symbol text;
ALTER TABLE currencies ADD COLUMN IF NOT EXISTS name text;

-- ACCOUNTS
CREATE TABLE IF NOT EXISTS accounts (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  name varchar(255) NOT NULL,
  type varchar(50) NOT NULL,
  currency char(3) NOT NULL,
  is_savings boolean NOT NULL DEFAULT false,
  opening_balance numeric(18,2) NOT NULL DEFAULT 0,
  include_in_networth boolean NOT NULL DEFAULT true,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE accounts ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS name varchar(255) NOT NULL;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS type varchar(50) NOT NULL;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS currency char(3) NOT NULL;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS is_savings boolean NOT NULL DEFAULT false;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS opening_balance numeric(18,2) NOT NULL DEFAULT 0;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS include_in_networth boolean NOT NULL DEFAULT true;
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE accounts ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_accounts_user_id'
  ) THEN
    ALTER TABLE accounts ADD CONSTRAINT fk_accounts_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_accounts_user_id ON accounts(user_id);

-- CATEGORIES
CREATE TABLE IF NOT EXISTS categories (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  name varchar(255) NOT NULL,
  parent_id uuid,
  type varchar(50) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE categories ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS name varchar(255) NOT NULL;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS parent_id uuid;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS type varchar(50) NOT NULL;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE categories ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_categories_user_id'
  ) THEN
    ALTER TABLE categories ADD CONSTRAINT fk_categories_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_categories_user_id ON categories(user_id);

-- GOALS
CREATE TABLE IF NOT EXISTS goals (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  name varchar(255) NOT NULL,
  target_amount numeric(18,2) NOT NULL,
  currency char(3) NOT NULL,
  account_id uuid,
  deadline date,
  note text,
  archived boolean NOT NULL DEFAULT false,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE goals ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS name varchar(255) NOT NULL;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS target_amount numeric(18,2) NOT NULL;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS currency char(3) NOT NULL;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS account_id uuid;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS deadline date;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS note text;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS archived boolean NOT NULL DEFAULT false;
ALTER TABLE goals ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE goals ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_goals_user_id'
  ) THEN
    ALTER TABLE goals ADD CONSTRAINT fk_goals_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_goals_account_id'
  ) THEN
    ALTER TABLE goals ADD CONSTRAINT fk_goals_account_id FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE SET NULL;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_goals_user_id ON goals(user_id);

-- TRANSACTIONS
CREATE TABLE IF NOT EXISTS transactions (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  account_id uuid NOT NULL,
  type varchar(50) NOT NULL,
  amount numeric(18,2) NOT NULL,
  currency char(3) NOT NULL,
  original_amount numeric(18,2),
  original_currency char(3),
  date date NOT NULL,
  settled_at timestamptz,
  category_id uuid,
  goal_id uuid,
  notes text,
  status varchar(50) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE transactions ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS account_id uuid NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS type varchar(50) NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS amount numeric(18,2) NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS currency char(3) NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS original_amount numeric(18,2);
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS original_currency char(3);
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS date date NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS settled_at timestamptz;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS category_id uuid;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS goal_id uuid;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS notes text;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS status varchar(50) NOT NULL;
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE transactions ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_transactions_user_id'
  ) THEN
    ALTER TABLE transactions ADD CONSTRAINT fk_transactions_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_transactions_account_id'
  ) THEN
    ALTER TABLE transactions ADD CONSTRAINT fk_transactions_account_id FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE;
  END IF;
END$$;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_transactions_category_id'
  ) THEN
    ALTER TABLE transactions ADD CONSTRAINT fk_transactions_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL;
  END IF;
END$$;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_transactions_goal_id'
  ) THEN
    ALTER TABLE transactions ADD CONSTRAINT fk_transactions_goal_id FOREIGN KEY (goal_id) REFERENCES goals(id) ON DELETE SET NULL;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_transactions_user_id ON transactions(user_id);
CREATE INDEX IF NOT EXISTS idx_transactions_account_id ON transactions(account_id);

-- BUDGETS
CREATE TABLE IF NOT EXISTS budgets (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL,
  category_id uuid NOT NULL,
  period varchar(50) NOT NULL,
  amount numeric(18,2) NOT NULL,
  start_date date NOT NULL,
  end_date date,
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE budgets ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS category_id uuid NOT NULL;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS period varchar(50) NOT NULL;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS amount numeric(18,2) NOT NULL;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS start_date date NOT NULL;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS end_date date;
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE budgets ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_budgets_user_id'
  ) THEN
    ALTER TABLE budgets ADD CONSTRAINT fk_budgets_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_budgets_category_id'
  ) THEN
    ALTER TABLE budgets ADD CONSTRAINT fk_budgets_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE;
  END IF;
END$$;

CREATE INDEX IF NOT EXISTS idx_budgets_user_id ON budgets(user_id);

-- final: ensure timezone and extensions (optional)
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp"; -- optional, not required if UUIDs provided by application

-- done
