# EMS Docker Management Makefile

.PHONY: help build up down logs clean dev prod restart status

# Default target
help:
	@echo "EMS Docker Management Commands:"
	@echo ""
	@echo "Production Commands:"
	@echo "  make prod          - Start production environment"
	@echo "  make build         - Build all Docker images"
	@echo "  make up            - Start all services"
	@echo "  make down          - Stop all services"
	@echo "  make restart       - Restart all services"
	@echo "  make status        - Show service status"
	@echo "  make logs          - Show all logs"
	@echo "  make logs-backend  - Show backend logs"
	@echo "  make logs-frontend - Show frontend logs"
	@echo "  make logs-db       - Show database logs"
	@echo ""
	@echo "Development Commands:"
	@echo "  make dev           - Start development environment"
	@echo "  make dev-down      - Stop development environment"
	@echo "  make dev-logs      - Show development logs"
	@echo ""
	@echo "Database Commands:"
	@echo "  make db-shell      - Access database shell"
	@echo "  make db-backup     - Backup database"
	@echo "  make db-restore    - Restore database (usage: make db-restore FILE=backup.sql)"
	@echo ""
	@echo "Cleanup Commands:"
	@echo "  make clean         - Remove containers and networks"
	@echo "  make clean-all     - Remove everything including volumes"
	@echo "  make clean-images  - Remove all images"

# Production commands
prod: build up

build:
	docker-compose build

up:
	docker-compose up -d

down:
	docker-compose down

restart:
	docker-compose restart

status:
	docker-compose ps

logs:
	docker-compose logs -f

logs-backend:
	docker-compose logs -f backend

logs-frontend:
	docker-compose logs -f frontend

logs-db:
	docker-compose logs -f mysql

# Development commands
dev:
	docker-compose -f docker-compose.dev.yml up -d

dev-down:
	docker-compose -f docker-compose.dev.yml down

dev-logs:
	docker-compose -f docker-compose.dev.yml logs -f

# Database commands
db-shell:
	docker-compose exec mysql mysql -u ems_user -p EMS

db-backup:
	docker-compose exec mysql mysqldump -u ems_user -p EMS > backup_$(shell date +%Y%m%d_%H%M%S).sql
	@echo "Database backed up to backup_$(shell date +%Y%m%d_%H%M%S).sql"

db-restore:
	@if [ -z "$(FILE)" ]; then echo "Usage: make db-restore FILE=backup.sql"; exit 1; fi
	docker-compose exec -T mysql mysql -u ems_user -p EMS < $(FILE)
	@echo "Database restored from $(FILE)"

# Cleanup commands
clean:
	docker-compose down
	docker-compose -f docker-compose.dev.yml down

clean-all: clean
	docker-compose down -v
	docker-compose -f docker-compose.dev.yml down -v
	docker system prune -f

clean-images:
	docker-compose down --rmi all
	docker-compose -f docker-compose.dev.yml down --rmi all
	docker image prune -a -f
