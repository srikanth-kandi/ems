-- Database initialization script for EMS
-- This script runs when the MySQL container starts for the first time

-- Create the EMS database if it doesn't exist
CREATE DATABASE IF NOT EXISTS EMS CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use the EMS database
USE EMS;

-- Create a user for the application if it doesn't exist
CREATE USER IF NOT EXISTS 'ems_user'@'%' IDENTIFIED BY 'ems_password123';

-- Grant all privileges on the EMS database to the ems_user
GRANT ALL PRIVILEGES ON EMS.* TO 'ems_user'@'%';

-- Grant all privileges on the EMS database to the root user
GRANT ALL PRIVILEGES ON EMS.* TO 'root'@'%';

-- Flush privileges to ensure they take effect
FLUSH PRIVILEGES;

-- Set timezone to UTC
SET time_zone = '+00:00';

-- Create a table to track initialization status
CREATE TABLE IF NOT EXISTS initialization_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    script_name VARCHAR(255) NOT NULL,
    executed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status ENUM('SUCCESS', 'FAILED') NOT NULL,
    message TEXT
);

-- Log successful initialization
INSERT INTO initialization_log (script_name, status, message) 
VALUES ('01-init-database.sql', 'SUCCESS', 'Database initialization completed successfully');

-- Display initialization status
SELECT 'Database initialization completed successfully' as status;
