# Litrater Service - Docker Setup

This document provides instructions for running the Litrater service using Docker and Docker Compose.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose
- At least 2GB of available RAM

## Quick Start

### Development Environment

1. **Clone the repository and navigate to the project root:**
   ```bash
   cd LitraterService
   ```

2. **Start the services:**
   ```bash
   docker-compose up -d
   ```

3. **Check the status:**
   ```bash
   docker-compose ps
   ```

4. **View logs:**
   ```bash
   docker-compose logs -f
   ```

5. **Access the application:**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health
   - PostgreSQL: localhost:5432

### Production Environment

1. **Create environment file:**
   ```bash
   cp env.example .env
   ```

2. **Edit the `.env` file with your production values:**
   ```bash
   # Update these values for production
   POSTGRES_PASSWORD=your-secure-password
   JWT_SECRET_KEY=your-super-secret-production-key
   ```

3. **Start production services:**
   ```bash
   docker-compose -f docker-compose.prod.yml up -d
   ```

## Configuration

### Environment Variables

The following environment variables can be configured:

| Variable | Description | Default |
|----------|-------------|---------|
| `POSTGRES_USER` | PostgreSQL username | `litrater_user` |
| `POSTGRES_PASSWORD` | PostgreSQL password | `litrater_password` |
| `POSTGRES_DB` | PostgreSQL database name | `litrater` |
| `JWT_SECRET_KEY` | JWT signing key | `your-super-secret-key-change-this-in-production` |
| `JWT_ISSUER` | JWT issuer | `LitraterAPI` |
| `JWT_AUDIENCE` | JWT audience | `LitraterUsers` |
| `JWT_EXPIRATION_MINUTES` | JWT token expiration | `60` |
| `ASPNETCORE_ENVIRONMENT` | .NET environment | `Development` |

### Database Connection

The application automatically connects to the PostgreSQL database using the following connection string:
```
Host=postgres;Port=5432;Database=litrater;Username=litrater_user;Password=litrater_password
```

## Docker Commands

### Development

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# Rebuild and start
docker-compose up -d --build

# View logs
docker-compose logs -f litrater-api
docker-compose logs -f postgres

# Execute commands in containers
docker-compose exec litrater-api dotnet ef database update
docker-compose exec postgres psql -U litrater_user -d litrater
```

### Production

```bash
# Start production services
docker-compose -f docker-compose.prod.yml up -d

# Stop production services
docker-compose -f docker-compose.prod.yml down

# View production logs
docker-compose -f docker-compose.prod.yml logs -f
```

## Database Management

### Initial Setup

The database is automatically initialized when the PostgreSQL container starts for the first time. Entity Framework migrations will be applied automatically in development mode.

### Manual Database Operations

```bash
# Apply migrations
docker-compose exec litrater-api dotnet ef database update

# Create a new migration
docker-compose exec litrater-api dotnet ef migrations add MigrationName

# Reset database
docker-compose exec litrater-api dotnet ef database drop --force
docker-compose exec litrater-api dotnet ef database update
```

### Database Backup

```bash
# Backup database
docker-compose exec postgres pg_dump -U litrater_user litrater > backup.sql

# Restore database
docker-compose exec -T postgres psql -U litrater_user litrater < backup.sql
```

## Troubleshooting

### Common Issues

1. **Port conflicts:**
   - If port 5000 or 5432 is already in use, modify the ports in `docker-compose.yml`

2. **Database connection issues:**
   - Ensure the PostgreSQL container is healthy: `docker-compose ps`
   - Check logs: `docker-compose logs postgres`

3. **Application startup issues:**
   - Check if the database is ready: `docker-compose logs litrater-api`
   - Verify environment variables are set correctly

4. **Permission issues:**
   - On Linux/Mac, you might need to run with `sudo` or add your user to the docker group

### Health Checks

Both services include health checks:

- **PostgreSQL:** Checks if the database is ready to accept connections
- **API:** Checks if the application responds to HTTP requests

### Logs

```bash
# View all logs
docker-compose logs

# Follow logs in real-time
docker-compose logs -f

# View specific service logs
docker-compose logs -f litrater-api
docker-compose logs -f postgres
```

## Security Considerations

1. **Change default passwords** in production
2. **Use strong JWT secret keys**
3. **Limit database access** to only necessary users
4. **Use environment variables** for sensitive configuration
5. **Regularly update** Docker images for security patches

## Performance Optimization

1. **Resource limits:** Add memory and CPU limits in production
2. **Database optimization:** Configure PostgreSQL for your workload
3. **Caching:** Consider adding Redis for caching
4. **Load balancing:** Use multiple API instances behind a load balancer

## Monitoring

The application includes health checks at `/health` that can be used for monitoring:

```bash
curl http://localhost:5000/health
```

This endpoint returns the health status of the application and its dependencies. 