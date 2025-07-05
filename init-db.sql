-- Initialize the Litrater database
-- This script runs when the PostgreSQL container starts for the first time

-- Create the database if it doesn't exist (PostgreSQL creates it automatically from environment variables)
-- Set up any initial database configuration if needed

-- Grant necessary permissions
GRANT ALL PRIVILEGES ON DATABASE litrater TO litrater_user;

-- You can add any additional initialization SQL here if needed
-- For example, creating extensions, setting up specific configurations, etc.

-- Example: Enable UUID extension if needed
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp"; 