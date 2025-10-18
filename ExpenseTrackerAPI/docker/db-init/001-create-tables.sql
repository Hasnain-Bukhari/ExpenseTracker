-- SQL initialization script for Postgres
CREATE TABLE IF NOT EXISTS users (
  id uuid PRIMARY KEY,
  email varchar(320) NOT NULL UNIQUE,
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

CREATE TABLE IF NOT EXISTS refresh_tokens (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  token_hash text NOT NULL,
  device_info text,
  expires_at timestamptz NOT NULL,
  created_at timestamptz NOT NULL DEFAULT now(),
  revoked_at timestamptz
);

CREATE TABLE IF NOT EXISTS password_reset_tokens (
  id uuid PRIMARY KEY,
  user_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE,
  token_hash text NOT NULL,
  expires_at timestamptz NOT NULL,
  used boolean NOT NULL DEFAULT false,
  created_at timestamptz NOT NULL DEFAULT now()
);

-- Indexes
CREATE INDEX IF NOT EXISTS idx_users_normalized_email ON users(normalized_email);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);
CREATE INDEX IF NOT EXISTS idx_password_reset_tokens_user_id ON password_reset_tokens(user_id);
