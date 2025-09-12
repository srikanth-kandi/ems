#!/bin/bash

# Comprehensive Database Seeding Service for EMS
# This script provides multiple seeding options and comprehensive data generation

set -e

# Configuration
API_URL="${API_URL:-http://backend:5000}"
MAX_RETRIES=60
RETRY_INTERVAL=5
SEED_TYPE="${SEED_TYPE:-seed}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if API is ready
check_api_health() {
    local response
    response=$(curl -f -s "$API_URL/health" 2>/dev/null)
    if [ $? -eq 0 ] && echo "$response" | grep -q "Healthy"; then
        return 0
    else
        return 1
    fi
}

# Function to get authentication token
get_auth_token() {
    local response
    log_info "Attempting to get authentication token..."
    
    response=$(curl -s -X POST "$API_URL/api/auth/login" \
        -H "Content-Type: application/json" \
        -d '{"username":"admin","password":"admin123"}' 2>/dev/null)
    
    if [ $? -eq 0 ] && [ -n "$response" ]; then
        local token=$(echo "$response" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
        if [ -n "$token" ]; then
            log_success "Authentication token obtained successfully"
            echo "$token"
        else
            log_warning "Could not extract token from response"
            echo ""
        fi
    else
        log_warning "Failed to get authentication token"
        echo ""
    fi
}

# Function to make API call with authentication
make_api_call() {
    local method=$1
    local endpoint=$2
    local token=$3
    local description=$4
    
    log_info "$description"
    
    local response
    if [ -n "$token" ]; then
        response=$(curl -s -X "$method" "$API_URL$endpoint" \
            -H "Content-Type: application/json" \
            -H "Authorization: Bearer $token" 2>/dev/null)
    else
        response=$(curl -s -X "$method" "$API_URL$endpoint" \
            -H "Content-Type: application/json" 2>/dev/null)
    fi
    
    local exit_code=$?
    if [ $exit_code -eq 0 ]; then
        log_success "API call successful: $description"
        echo "$response"
    else
        log_error "API call failed: $description (exit code: $exit_code)"
        echo ""
    fi
}

# Function to get database status
get_database_status() {
    local token=$1
    local response
    response=$(make_api_call "GET" "/api/seed/status" "$token" "Getting database status")
    
    if [ -n "$response" ]; then
        log_info "Database Status:"
        echo "$response" | jq '.' 2>/dev/null || echo "$response"
    else
        log_warning "Could not retrieve database status"
    fi
}

# Function to seed database
seed_database() {
    local token=$1
    local seed_type=$2
    
    case $seed_type in
        "seed")
            make_api_call "POST" "/api/seed/seed" "$token" "Seeding database with initial data"
            ;;
        "reseed")
            make_api_call "POST" "/api/seed/reseed" "$token" "Reseeding database with fresh data"
            ;;
        "clear")
            make_api_call "DELETE" "/api/seed/clear" "$token" "Clearing all data from database"
            ;;
        *)
            log_error "Unknown seed type: $seed_type"
            return 1
            ;;
    esac
}

# Main execution
main() {
    log_info "Starting EMS Database Seeding Service"
    log_info "API URL: $API_URL"
    log_info "Seed Type: $SEED_TYPE"
    
    # Wait for API to be ready
    log_info "Waiting for backend API to be ready..."
    local retry_count=0
    
    while [ $retry_count -lt $MAX_RETRIES ]; do
        if check_api_health; then
            log_success "Backend API is ready!"
            break
        fi
        
        retry_count=$((retry_count + 1))
        log_info "Attempt $retry_count/$MAX_RETRIES: API not ready, waiting $RETRY_INTERVAL seconds..."
        sleep $RETRY_INTERVAL
    done
    
    if [ $retry_count -eq $MAX_RETRIES ]; then
        log_error "Backend API did not become ready within expected time"
        exit 1
    fi
    
    # Get authentication token
    local token
    token=$(get_auth_token)
    
    # Get initial database status
    log_info "Getting initial database status..."
    get_database_status "$token"
    
    # Perform seeding operation
    log_info "Performing seeding operation: $SEED_TYPE"
    local seed_response
    seed_response=$(seed_database "$token" "$SEED_TYPE")
    
    if [ -n "$seed_response" ]; then
        log_info "Seed operation response:"
        echo "$seed_response" | jq '.' 2>/dev/null || echo "$seed_response"
        
        # Check if seeding was successful
        if echo "$seed_response" | grep -q "successfully\|seeded\|reseeded\|cleared"; then
            log_success "Database seeding operation completed successfully!"
        elif echo "$seed_response" | grep -q "already contains data"; then
            log_warning "Database already contains data, skipping seeding"
        else
            log_warning "Seeding operation completed with warnings or unknown response"
        fi
    else
        log_error "Seeding operation failed - no response received"
        exit 1
    fi
    
    # Get final database status
    log_info "Getting final database status..."
    get_database_status "$token"
    
    log_success "Database seeding service completed successfully!"
}

# Run main function
main "$@"
