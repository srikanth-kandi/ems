#!/bin/bash

# Database seeding script for EMS
# This script waits for the backend API to be ready and then seeds the database

set -e

echo "Starting database seeding process..."

# Configuration
API_URL="http://backend:5000"
MAX_RETRIES=30
RETRY_INTERVAL=10

# Function to check if API is ready
check_api_health() {
    curl -f -s "$API_URL/health" > /dev/null 2>&1
}

# Function to get authentication token
get_auth_token() {
    local response
    response=$(curl -s -X POST "$API_URL/api/auth/login" \
        -H "Content-Type: application/json" \
        -d '{"username":"admin","password":"admin123"}' 2>/dev/null)
    
    if [ $? -eq 0 ] && [ -n "$response" ]; then
        echo "$response" | grep -o '"token":"[^"]*"' | cut -d'"' -f4
    else
        echo ""
    fi
}

# Function to seed database
seed_database() {
    local token=$1
    local endpoint=$2
    
    echo "Seeding database via $endpoint..."
    
    if [ -n "$token" ]; then
        curl -s -X POST "$API_URL$endpoint" \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $token"
    else
        curl -s -X POST "$API_URL$endpoint" \
            -H "Content-Type: application/json"
    fi
}

# Wait for API to be ready
echo "Waiting for backend API to be ready..."
retry_count=0

while [ $retry_count -lt $MAX_RETRIES ]; do
    if check_api_health; then
        echo "Backend API is ready!"
        break
    fi
    
    retry_count=$((retry_count + 1))
    echo "Attempt $retry_count/$MAX_RETRIES: API not ready, waiting $RETRY_INTERVAL seconds..."
    sleep $RETRY_INTERVAL
done

if [ $retry_count -eq $MAX_RETRIES ]; then
    echo "ERROR: Backend API did not become ready within expected time"
    exit 1
fi

# Get authentication token
echo "Getting authentication token..."
TOKEN=$(get_auth_token)

if [ -n "$TOKEN" ]; then
    echo "Authentication token obtained successfully"
else
    echo "WARNING: Could not obtain authentication token, proceeding without authentication"
fi

# Seed the database
echo "Starting database seeding..."

# Try to seed first (will only seed if database is empty)
SEED_RESPONSE=$(seed_database "$TOKEN" "/api/seed/seed")
echo "Seed response: $SEED_RESPONSE"

# Check if seeding was successful or if database already has data
if echo "$SEED_RESPONSE" | grep -q "already contains data"; then
    echo "Database already contains data, skipping seeding"
elif echo "$SEED_RESPONSE" | grep -q "seeded successfully"; then
    echo "Database seeded successfully with initial data"
else
    echo "Attempting to reseed database with fresh data..."
    RESEED_RESPONSE=$(seed_database "$TOKEN" "/api/seed/reseed")
    echo "Reseed response: $RESEED_RESPONSE"
    
    if echo "$RESEED_RESPONSE" | grep -q "reseeded successfully"; then
        echo "Database reseeded successfully with fresh data"
    else
        echo "ERROR: Database seeding failed"
        exit 1
    fi
fi

# Get final database status
echo "Getting final database status..."
STATUS_RESPONSE=$(curl -s -X GET "$API_URL/api/seed/status" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" 2>/dev/null)

if [ -n "$STATUS_RESPONSE" ]; then
    echo "Database status: $STATUS_RESPONSE"
else
    echo "Could not retrieve database status"
fi

echo "Database seeding process completed successfully!"
