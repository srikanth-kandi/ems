# Employee Management System - Deployment Guide

## 📋 Overview

This comprehensive deployment guide covers multiple deployment options for the Employee Management System, including Docker containerization, Oracle Cloud Infrastructure, and local development setups. The guide provides step-by-step instructions for production-ready deployments with security, monitoring, and maintenance considerations.

## 🏗️ Architecture Overview

### Production Environment
- **Frontend**: `https://ems.srikanthkandi.tech` (React.js SPA)
- **Backend API**: `https://api.ems.srikanthkandi.tech` (ASP.NET Web API)
- **Database**: Oracle Cloud MySQL Database Service (Always Free)
- **Server**: Ubuntu 22.04 LTS (VM.Standard.E2.1.Micro - Always Free)
- **Load Balancer**: Nginx with SSL termination
- **SSL**: Let's Encrypt certificates with auto-renewal

### Development Environment
- **Frontend**: `http://localhost:3000` (Vite dev server)
- **Backend API**: `http://localhost:5000` (ASP.NET Web API)
- **Database**: MySQL 8.0 (Docker container)
- **Hot Reload**: Enabled for both frontend and backend

### Docker Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                    Docker Compose                          │
│  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌───────┐ │
│  │   Frontend  │ │   Backend   │ │   MySQL     │ │ Redis │ │
│  │   (Nginx)   │ │   (.NET)    │ │  (Database) │ │(Cache)│ │
│  │   Port 3000 │ │   Port 5000 │ │  Port 3306  │ │6379   │ │
│  └─────────────┘ └─────────────┘ └─────────────┘ └───────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 🐳 Docker Deployment (Recommended)

### Quick Start with Docker

The easiest way to deploy the EMS is using Docker Compose. This method handles all dependencies and configurations automatically.

#### Prerequisites
- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2.0+
- At least 4GB RAM and 2GB disk space

#### Production Deployment
```bash
# Clone the repository
git clone <repository-url>
cd ems

# Start all services
docker-compose up -d

# Check service status
docker-compose ps

# View logs
docker-compose logs -f
```

#### Development Deployment
```bash
# Start development environment with hot reload
docker-compose -f docker-compose.dev.yml up -d

# View development logs
docker-compose -f docker-compose.dev.yml logs -f
```

#### Database Seeding
```bash
# Start with comprehensive database seeding
docker-compose -f docker-compose.yml -f docker-compose.seed.yml up -d

# Or for development with seeding
docker-compose -f docker-compose.dev.yml -f docker-compose.dev-seed.yml up -d
```

### Docker Services Configuration

#### Frontend Service
- **Image**: Custom React build with Nginx
- **Port**: 3000 (mapped to host)
- **Features**: Production-optimized build, static file serving
- **Health Check**: HTTP GET on `/`

#### Backend Service
- **Image**: ASP.NET Web API (.NET 8)
- **Port**: 5000 (mapped to host)
- **Features**: JWT authentication, API endpoints, Swagger documentation
- **Health Check**: HTTP GET on `/health`

#### MySQL Service
- **Image**: mysql:8.0
- **Port**: 3306 (mapped to host)
- **Database**: EMS
- **Credentials**: ems_user / ems_password123
- **Persistence**: Docker volume for data persistence

#### Redis Service (Optional)
- **Image**: redis:7-alpine
- **Port**: 6379 (mapped to host)
- **Purpose**: Caching and session storage
- **Persistence**: Redis data persistence enabled

### Environment Variables

Create a `.env` file in the project root:

```env
# Database Configuration
MYSQL_ROOT_PASSWORD=root_password_123
MYSQL_DATABASE=EMS
MYSQL_USER=ems_user
MYSQL_PASSWORD=ems_password123

# API Configuration
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000
JWT_KEY=your-super-secret-jwt-key-here-make-it-long-and-random
JWT_ISSUER=http://localhost:5000
JWT_AUDIENCE=http://localhost:3000

# Frontend Configuration
VITE_API_URL=http://localhost:5000
```

### Docker Commands Reference

```bash
# Build all services
docker-compose build

# Start services in background
docker-compose up -d

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# View service logs
docker-compose logs [service-name]

# Execute commands in running container
docker-compose exec backend dotnet ef database update
docker-compose exec mysql mysql -u ems_user -p EMS

# Scale services (if needed)
docker-compose up -d --scale backend=2
```

## ☁️ Oracle Cloud Infrastructure Setup

### 1. Create Oracle Cloud Account
1. Sign up for Oracle Cloud Free Tier
2. Verify your account
3. Access the Oracle Cloud Console

### 2. Create Virtual Machine Instance

#### Instance Configuration
- **Shape**: VM.Standard.E2.1.Micro (Always Free)
- **Image**: Ubuntu 22.04 LTS
- **Boot Volume**: 50 GB (Always Free)
- **Networking**: Default VCN with public subnet

#### Security Rules
Create ingress rules for:
- **SSH (22)**: Your IP only
- **HTTP (80)**: 0.0.0.0/0
- **HTTPS (443)**: 0.0.0.0/0

### 3. MySQL Database Service

#### Create MySQL Database
1. Navigate to MySQL Database Service
2. Create a new MySQL Database System
3. **Configuration**:
   - **Shape**: MySQL.Standalone.1.1 (Always Free)
   - **Storage**: 25 GB (Always Free)
   - **Backup**: Enable automated backups
   - **High Availability**: Disabled (Always Free limitation)

#### Database Configuration
- **Database Name**: `EMS`
- **Username**: `ems_user`
- **Password**: Generate strong password
- **Character Set**: `utf8mb4`
- **Collation**: `utf8mb4_unicode_ci`

#### Network Configuration
- **Subnet**: Private subnet
- **Security List**: Allow MySQL port (3306) from application subnet

## Server Setup

### 1. Connect to Instance
```bash
ssh -i your-key.pem ubuntu@your-instance-ip
```

### 2. Update System
```bash
sudo apt update && sudo apt upgrade -y
```

### 3. Install .NET 8 Runtime
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET 8 runtime
sudo apt update
sudo apt install -y dotnet-runtime-8.0
```

### 4. Install Nginx
```bash
sudo apt install -y nginx
sudo systemctl start nginx
sudo systemctl enable nginx
```

### 5. Install Node.js (for frontend build)
```bash
# Install Node.js 18
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt install -y nodejs
```

### 6. Install Certbot (for SSL)
```bash
sudo apt install -y certbot python3-certbot-nginx
```

## Application Deployment

### 1. Backend Deployment

#### Create Application Directory
```bash
sudo mkdir -p /var/www/ems-api
sudo chown ubuntu:ubuntu /var/www/ems-api
```

#### Deploy Application Files
```bash
# Copy application files to server
scp -i your-key.pem -r backend/ ubuntu@your-instance-ip:/var/www/ems-api/
```

#### Build and Publish
```bash
cd /var/www/ems-api/backend
dotnet publish EMS.API/EMS.API.csproj -c Release -o /var/www/ems-api/publish
```

#### Create Systemd Service
```bash
sudo nano /etc/systemd/system/ems-api.service
```

**Service Configuration:**
```ini
[Unit]
Description=EMS API
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/ems-api/publish
ExecStart=/usr/bin/dotnet /var/www/ems-api/publish/EMS.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=ems-api
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

#### Start Service
```bash
sudo systemctl daemon-reload
sudo systemctl start ems-api
sudo systemctl enable ems-api
```

### 2. Frontend Deployment

#### Build Frontend
```bash
cd /var/www/ems-api/frontend
npm install
npm run build
```

#### Configure Nginx for Frontend
```bash
sudo nano /etc/nginx/sites-available/ems-frontend
```

**Frontend Configuration:**
```nginx
server {
    listen 80;
    server_name ems.srikanthkandi.tech;
    root /var/www/ems-api/frontend/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### 3. API Configuration

#### Configure Nginx for API
```bash
sudo nano /etc/nginx/sites-available/ems-api
```

**API Configuration:**
```nginx
server {
    listen 80;
    server_name api.ems.srikanthkandi.tech;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

#### Enable Sites
```bash
sudo ln -s /etc/nginx/sites-available/ems-frontend /etc/nginx/sites-enabled/
sudo ln -s /etc/nginx/sites-available/ems-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

## Database Configuration

### 1. Connection String
Create production configuration:
```bash
sudo nano /var/www/ems-api/publish/appsettings.Production.json
```

**Production Configuration:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-mysql-endpoint;Database=EMS;Uid=ems_user;Pwd=your-password;SslMode=Required;"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here-make-it-long-and-random",
    "Issuer": "https://api.ems.srikanthkandi.tech",
    "Audience": "https://ems.srikanthkandi.tech",
    "ExpiryMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. Run Database Migrations
```bash
cd /var/www/ems-api/publish
dotnet ef database update
```

## SSL Certificate Setup

### 1. Obtain SSL Certificates
```bash
sudo certbot --nginx -d ems.srikanthkandi.tech -d api.ems.srikanthkandi.tech
```

### 2. Auto-renewal Setup
```bash
sudo crontab -e
```

Add the following line:
```bash
0 12 * * * /usr/bin/certbot renew --quiet
```

## Domain Configuration

### 1. DNS Setup
Configure your domain DNS records:
- **A Record**: `ems.srikanthkandi.tech` → Your Oracle Cloud instance IP
- **A Record**: `api.ems.srikanthkandi.tech` → Your Oracle Cloud instance IP

### 2. Domain Verification
Wait for DNS propagation (up to 48 hours) and verify:
```bash
nslookup ems.srikanthkandi.tech
nslookup api.ems.srikanthkandi.tech
```

## Monitoring and Logging

### 1. Application Logs
```bash
# View API logs
sudo journalctl -u ems-api -f

# View Nginx logs
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

### 2. System Monitoring
```bash
# Install monitoring tools
sudo apt install -y htop iotop

# Monitor system resources
htop
```

### 3. Database Monitoring
Monitor MySQL performance through Oracle Cloud Console:
- CPU utilization
- Memory usage
- Storage usage
- Connection count

## Backup Strategy

### 1. Database Backups
Oracle Cloud MySQL provides automated backups:
- **Retention**: 7 days (Always Free)
- **Backup Window**: Configure during setup
- **Point-in-time Recovery**: Available

### 2. Application Backups
```bash
# Create backup script
sudo nano /usr/local/bin/backup-ems.sh
```

**Backup Script:**
```bash
#!/bin/bash
BACKUP_DIR="/var/backups/ems"
DATE=$(date +%Y%m%d_%H%M%S)

# Create backup directory
mkdir -p $BACKUP_DIR

# Backup application files
tar -czf $BACKUP_DIR/ems-app-$DATE.tar.gz /var/www/ems-api

# Keep only last 7 days of backups
find $BACKUP_DIR -name "ems-app-*.tar.gz" -mtime +7 -delete

echo "Backup completed: ems-app-$DATE.tar.gz"
```

```bash
sudo chmod +x /usr/local/bin/backup-ems.sh
```

### 3. Automated Backups
```bash
sudo crontab -e
```

Add backup schedule:
```bash
0 2 * * * /usr/local/bin/backup-ems.sh
```

## Security Hardening

### 1. Firewall Configuration
```bash
# Install UFW
sudo apt install -y ufw

# Configure firewall
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

### 2. SSH Security
```bash
# Edit SSH configuration
sudo nano /etc/ssh/sshd_config
```

**Security Settings:**
```
PermitRootLogin no
PasswordAuthentication no
PubkeyAuthentication yes
Port 22
```

```bash
sudo systemctl restart ssh
```

### 3. Application Security
- Use strong JWT secrets
- Enable HTTPS only
- Configure CORS properly
- Implement rate limiting
- Regular security updates

## Performance Optimization

### 1. Nginx Optimization
```bash
sudo nano /etc/nginx/nginx.conf
```

**Performance Settings:**
```nginx
worker_processes auto;
worker_connections 1024;

gzip on;
gzip_vary on;
gzip_min_length 1024;
gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;
```

### 2. Application Optimization
- Enable response compression
- Configure caching headers
- Optimize database queries
- Use connection pooling

## Troubleshooting

### Common Issues

#### 1. Application Won't Start
```bash
# Check service status
sudo systemctl status ems-api

# Check logs
sudo journalctl -u ems-api -n 50
```

#### 2. Database Connection Issues
```bash
# Test database connection
mysql -h your-mysql-endpoint -u ems_user -p EMS
```

#### 3. SSL Certificate Issues
```bash
# Check certificate status
sudo certbot certificates

# Renew certificates manually
sudo certbot renew
```

#### 4. Nginx Configuration Issues
```bash
# Test configuration
sudo nginx -t

# Reload configuration
sudo systemctl reload nginx
```

## Maintenance

### 1. Regular Updates
```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Update application
# (Deploy new version following deployment steps)
```

### 2. Log Rotation
```bash
# Configure log rotation
sudo nano /etc/logrotate.d/ems-api
```

**Log Rotation Configuration:**
```
/var/log/ems-api/*.log {
    daily
    missingok
    rotate 30
    compress
    delaycompress
    notifempty
    create 644 www-data www-data
    postrotate
        systemctl reload ems-api
    endscript
}
```

### 3. Health Checks
Create health check endpoint monitoring:
```bash
# Create health check script
sudo nano /usr/local/bin/health-check.sh
```

**Health Check Script:**
```bash
#!/bin/bash
API_URL="https://api.ems.srikanthkandi.tech/health"
FRONTEND_URL="https://ems.srikanthkandi.tech"

# Check API health
if curl -f -s $API_URL > /dev/null; then
    echo "API is healthy"
else
    echo "API is down"
    # Send alert notification
fi

# Check frontend
if curl -f -s $FRONTEND_URL > /dev/null; then
    echo "Frontend is healthy"
else
    echo "Frontend is down"
    # Send alert notification
fi
```

## Scaling Considerations

### 1. Horizontal Scaling
- Use load balancer for multiple instances
- Implement session affinity
- Use shared database

### 2. Database Scaling
- Read replicas for read-heavy workloads
- Connection pooling
- Query optimization

### 3. Caching
- Redis for session storage
- CDN for static assets
- Application-level caching

## Cost Optimization

### Always Free Tier Limits
- **Compute**: 1 VM.Standard.E2.1.Micro instance
- **Storage**: 200 GB total
- **Database**: 1 MySQL.Standalone.1.1 instance
- **Bandwidth**: 10 TB/month

### Monitoring Usage
- Monitor resource usage in Oracle Cloud Console
- Set up billing alerts
- Optimize resource utilization

## 🔧 Troubleshooting & Maintenance

### Common Deployment Issues

#### Docker Issues

**Problem**: Container won't start
```bash
# Check container logs
docker-compose logs [service-name]

# Rebuild containers
docker-compose build --no-cache
docker-compose up -d

# Check resource usage
docker stats
```

**Problem**: Port conflicts
```bash
# Check what's using the ports
netstat -tulpn | grep :3000
netstat -tulpn | grep :5000
netstat -tulpn | grep :3306

# Change ports in docker-compose.yml if needed
```

**Problem**: Database connection issues
```bash
# Check if MySQL is ready
docker-compose exec mysql mysqladmin ping -h localhost

# Check database logs
docker-compose logs mysql

# Reset database
docker-compose down -v
docker-compose up -d
```

#### Oracle Cloud Issues

**Problem**: Instance won't start
- Check Oracle Cloud Console for instance status
- Verify security list rules
- Check if Always Free tier limits are exceeded

**Problem**: SSL certificate issues
```bash
# Check certificate status
sudo certbot certificates

# Renew certificates manually
sudo certbot renew --dry-run
sudo certbot renew

# Check Nginx configuration
sudo nginx -t
sudo systemctl reload nginx
```

**Problem**: Database connection from application
- Verify security list allows MySQL port (3306)
- Check connection string in appsettings.Production.json
- Ensure database is accessible from application subnet

### Performance Optimization

#### Database Optimization
```sql
-- Check slow queries
SHOW PROCESSLIST;

-- Optimize tables
OPTIMIZE TABLE employees, departments, attendance;

-- Check index usage
EXPLAIN SELECT * FROM employees WHERE department_id = 1;
```

#### Application Optimization
- Enable response compression in Nginx
- Configure caching headers
- Use connection pooling
- Monitor memory usage

#### Monitoring Commands
```bash
# Check system resources
htop
df -h
free -h

# Check application logs
sudo journalctl -u ems-api -f
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log

# Check database performance
docker-compose exec mysql mysql -u ems_user -p EMS -e "SHOW PROCESSLIST;"
```

### Backup and Recovery

#### Database Backup
```bash
# Create backup
docker-compose exec mysql mysqldump -u ems_user -p EMS > backup_$(date +%Y%m%d_%H%M%S).sql

# Restore backup
docker-compose exec -T mysql mysql -u ems_user -p EMS < backup_file.sql
```

#### Application Backup
```bash
# Backup application files
tar -czf ems-backup-$(date +%Y%m%d_%H%M%S).tar.gz /var/www/ems-api

# Backup with Docker volumes
docker run --rm -v ems_mysql_data:/data -v $(pwd):/backup alpine tar czf /backup/mysql-backup.tar.gz -C /data .
```

### Security Hardening

#### Firewall Configuration
```bash
# Install UFW
sudo apt install ufw

# Configure firewall
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw enable
```

#### SSL/TLS Security
- Use strong cipher suites
- Enable HSTS headers
- Regular certificate renewal
- Monitor SSL Labs rating

#### Application Security
- Regular security updates
- Strong JWT secrets
- Input validation
- SQL injection prevention
- XSS protection

### Monitoring and Alerting

#### Health Checks
```bash
# API health check
curl -f http://localhost:5000/health

# Frontend health check
curl -f http://localhost:3000

# Database health check
docker-compose exec mysql mysqladmin ping -h localhost
```

#### Log Monitoring
```bash
# Set up log rotation
sudo nano /etc/logrotate.d/ems-api

# Monitor error rates
grep "ERROR" /var/log/ems-api/application.log | tail -10

# Check access patterns
awk '{print $1}' /var/log/nginx/access.log | sort | uniq -c | sort -nr
```

### Scaling Considerations

#### Horizontal Scaling
- Use load balancer for multiple instances
- Implement session affinity
- Use shared database
- Consider microservices architecture

#### Vertical Scaling
- Upgrade instance size
- Increase database resources
- Optimize application code
- Use caching strategies

## 📊 Cost Optimization

### Always Free Tier Limits
- **Compute**: 1 VM.Standard.E2.1.Micro instance
- **Storage**: 200 GB total
- **Database**: 1 MySQL.Standalone.1.1 instance
- **Bandwidth**: 10 TB/month

### Monitoring Usage
- Check resource usage in Oracle Cloud Console
- Set up billing alerts
- Optimize resource utilization
- Use cost analysis tools

## 🚀 Future Enhancements

### CI/CD Pipeline
- GitHub Actions for automated deployment
- Automated testing before deployment
- Blue-green deployment strategy
- Rollback capabilities

### Monitoring Stack
- Prometheus for metrics collection
- Grafana for visualization
- ELK Stack for log aggregation
- AlertManager for notifications

### Security Enhancements
- WAF (Web Application Firewall)
- DDoS protection
- Security scanning
- Compliance monitoring

---

This deployment guide provides a comprehensive approach to deploying the Employee Management System on Oracle Cloud Infrastructure using the Always Free tier services, with additional Docker deployment options and extensive troubleshooting guidance.
