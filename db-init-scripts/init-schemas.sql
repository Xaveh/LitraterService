-- Initialize schemas for Litrater application
-- This script creates the required schemas in the litrater database

-- Create schema for the web API
CREATE SCHEMA IF NOT EXISTS litrater_web_api;

-- Create schema for Keycloak
CREATE SCHEMA IF NOT EXISTS litrater_keycloak;

-- Grant usage and create privileges to the litrater_user on both schemas
GRANT USAGE, CREATE ON SCHEMA litrater_web_api TO litrater_user;
GRANT USAGE, CREATE ON SCHEMA litrater_keycloak TO litrater_user;

-- Set default privileges for future tables in both schemas
ALTER DEFAULT PRIVILEGES IN SCHEMA litrater_web_api GRANT ALL ON TABLES TO litrater_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA litrater_web_api GRANT ALL ON SEQUENCES TO litrater_user;

ALTER DEFAULT PRIVILEGES IN SCHEMA litrater_keycloak GRANT ALL ON TABLES TO litrater_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA litrater_keycloak GRANT ALL ON SEQUENCES TO litrater_user; 