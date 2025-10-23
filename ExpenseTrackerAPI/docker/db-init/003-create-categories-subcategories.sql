-- filepath: docker/db-init/003-create-categories-subcategories.sql
-- Idempotent DDL to add categories and sub_categories (safe to run multiple times)

-- Ensure pgcrypto (for gen_random_uuid) is available (no-op if already enabled)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create enum type for category type if it does not exist
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'category_type_enum') THEN
    CREATE TYPE category_type_enum AS ENUM ('expense','income');
  END IF;
END$$;

-- Categories table
CREATE TABLE IF NOT EXISTS categories (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id uuid NOT NULL,
  name varchar(255) NOT NULL,
  type category_type_enum NOT NULL DEFAULT 'expense',
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

-- Add missing columns if table exists with different schema
ALTER TABLE categories ADD COLUMN IF NOT EXISTS user_id uuid NOT NULL;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS name varchar(255) NOT NULL;
ALTER TABLE categories ADD COLUMN IF NOT EXISTS type category_type_enum NOT NULL DEFAULT 'expense';
ALTER TABLE categories ADD COLUMN IF NOT EXISTS description varchar(255);
ALTER TABLE categories ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE categories ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

-- Ensure FK to users (add constraint if missing)
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_categories_user_id'
  ) THEN
    ALTER TABLE categories ADD CONSTRAINT fk_categories_user_id FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE;
  END IF;
END$$;

-- Unique index to prevent duplicate category name (case-insensitive) per user and type
CREATE UNIQUE INDEX IF NOT EXISTS ux_categories_userid_name_type ON categories (user_id, lower(name), type);
CREATE INDEX IF NOT EXISTS idx_categories_user_id ON categories(user_id);

-- SubCategories table
CREATE TABLE IF NOT EXISTS sub_categories (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  category_id uuid NOT NULL,
  name varchar(255) NOT NULL,
  description varchar(255),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now()
);

ALTER TABLE sub_categories ADD COLUMN IF NOT EXISTS category_id uuid NOT NULL;
ALTER TABLE sub_categories ADD COLUMN IF NOT EXISTS name varchar(255) NOT NULL;
ALTER TABLE sub_categories ADD COLUMN IF NOT EXISTS description varchar(255);
ALTER TABLE sub_categories ADD COLUMN IF NOT EXISTS created_at timestamptz NOT NULL DEFAULT now();
ALTER TABLE sub_categories ADD COLUMN IF NOT EXISTS updated_at timestamptz NOT NULL DEFAULT now();

-- FK category -> categories
DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'fk_subcategories_category_id'
  ) THEN
    ALTER TABLE sub_categories ADD CONSTRAINT fk_subcategories_category_id FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE;
  END IF;
END$$;

-- Unique index to prevent duplicate subcategory name (case-insensitive) per category
CREATE UNIQUE INDEX IF NOT EXISTS ux_subcategories_categoryid_name ON sub_categories (category_id, lower(name));
CREATE INDEX IF NOT EXISTS idx_sub_categories_category_id ON sub_categories(category_id);

-- Done
