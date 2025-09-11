# Docker Setup Guide

This guide explains how to run the EMS (Employee Management System) application using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2.0 or higher
- At least 4GB of available RAM
- At least 2GB of available disk space

## Quick Start

### Production Environment

1. **Clone the repository and navigate to the project root:**
   ```bash
   git clone <repository-url>
   cd ems
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

3. **Access the application:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - MySQL Database: localhost:3306

4. **Stop all services:**
   ```bash
   docker-compose down
   ```

### Development Environment

1. **Start development services:**
   ```bash
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. **Access the application:**
   - Frontend: http://localhost:3000 (with hot reload)
   - Backend API: http://localhost:5000
   - MySQL Database: localhost:3306

3. **Stop development services:**
   ```bash
   docker-compose -f docker-compose.dev.yml down
   ```

## Services Overview

### MySQL Database
- **Image:** mysql:8.0
- **Port:** 3306
- **Database:** EMS
- **Username:** ems_user
- **Password:** ems_password123
- **Root Password:** root_password_123

### Backend API (.NET 8)
- **Port:** 5000
- **Environment:** Production/Development
- **Health Check:** http://localhost:5000/health
- **Features:**
  - JWT Authentication
  - Entity Framework Core with MySQL
  - Swagger API Documentation
  - CORS enabled for frontend

### Frontend (React + Vite)
- **Port:** 3000 (production) / 3000 (development)
- **Features:**
  - Nginx web server (production)
  - Hot reload (development)
  - Material-UI components
  - Axios for API calls

### Redis (Optional)
- **Port:** 6379
- **Purpose:** Caching and session management
- **Data persistence:** Enabled

## Docker Commands

### Building Images

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build backend
docker-compose build frontend

# Build with no cache
docker-compose build --no-cache
```

### Managing Services

```bash
# Start services in background
docker-compose up -d

# Start specific service
docker-compose up -d backend

# View logs
docker-compose logs
docker-compose logs backend
docker-compose logs frontend

# Follow logs in real-time
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### Database Management

```bash
# Access MySQL container
docker-compose exec mysql mysql -u ems_user -p EMS

# Run database migrations
docker-compose exec backend dotnet ef database update

# Backup database
docker-compose exec mysql mysqldump -u ems_user -p EMS > backup.sql

# Restore database
docker-compose exec -T mysql mysql -u ems_user -p EMS < backup.sql
```

### Development Commands

```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# View development logs
docker-compose -f docker-compose.dev.yml logs -f

# Stop development environment
docker-compose -f docker-compose.dev.yml down

# Rebuild development images
docker-compose -f docker-compose.dev.yml build --no-cache
```

## Environment Variables

### Backend Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Production/Development) | Production |
| `ASPNETCORE_URLS` | API URLs | http://+:5000 |
| `ConnectionStrings__DefaultConnection` | MySQL connection string | Auto-configured |
| `Jwt__Key` | JWT secret key | Auto-generated |
| `Jwt__Issuer` | JWT issuer | http://localhost:5000 |
| `Jwt__Audience` | JWT audience | http://localhost:3000 |
| `Jwt__ExpiryMinutes` | JWT expiry time | 60 |

### Database Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `MYSQL_ROOT_PASSWORD` | MySQL root password | root_password_123 |
| `MYSQL_DATABASE` | Database name | EMS |
| `MYSQL_USER` | Database user | ems_user |
| `MYSQL_PASSWORD` | Database password | ems_password123 |

## Troubleshooting

### Common Issues

1. **Port conflicts:**
   - Ensure ports 3000, 5000, and 3306 are not in use
   - Change ports in docker-compose.yml if needed

2. **Database connection issues:**
   - Wait for MySQL to fully start (check health status)
   - Verify connection string in appsettings.json

3. **Frontend not loading:**
   - Check if backend is running and healthy
   - Verify nginx configuration

4. **Permission issues:**
   - Ensure Docker has proper permissions
   - On Linux, you might need to run with sudo

### Health Checks

```bash
# Check service health
docker-compose ps

# Check specific service health
docker inspect ems-backend | grep Health -A 10
docker inspect ems-mysql | grep Health -A 10
```

### Logs and Debugging

```bash
# View all logs
docker-compose logs

# View specific service logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs mysql

# View logs with timestamps
docker-compose logs -t

# View last 100 lines
docker-compose logs --tail=100
```

### Cleanup

```bash
# Remove all containers and networks
docker-compose down

# Remove all containers, networks, and volumes
docker-compose down -v

# Remove all images
docker-compose down --rmi all

# Remove everything (containers, networks, volumes, images)
docker system prune -a
```

## Security Considerations

1. **Change default passwords** in production
2. **Use environment files** for sensitive data
3. **Enable SSL/TLS** for production deployments
4. **Regularly update** Docker images
5. **Use secrets management** for sensitive data

## Performance Optimization

1. **Resource limits:** Add memory and CPU limits to services
2. **Multi-stage builds:** Already implemented for smaller images
3. **Caching:** Use Docker layer caching effectively
4. **Database optimization:** Tune MySQL settings for your workload

## Monitoring

Consider adding monitoring tools like:
- Prometheus + Grafana for metrics
- ELK Stack for logging
- Health check endpoints for uptime monitoring

## Support

For issues and questions:
1. Check the logs first
2. Verify all services are healthy
3. Check network connectivity between services
4. Review environment variables and configuration
