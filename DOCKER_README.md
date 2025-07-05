# Litrater Service - Docker Setup

Simple Docker setup for the Litrater service with PostgreSQL database.

## Quick Start

1. **Start the services:**
   ```bash
   docker-compose -f docker/docker-compose.yml up -d
   ```

2. **Access the application:**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health Check: http://localhost:5000/health
   - PostgreSQL: localhost:5432

3. **Stop the services:**
   ```bash
   docker-compose -f docker/docker-compose.yml down
   ```

## Configuration

### Environment Variables

Create a `.env` file in the project root:

| Variable | Description | Default |
|----------|-------------|---------|
| `POSTGRES_USER` | PostgreSQL username | `litrater_user` |
| `POSTGRES_PASSWORD` | PostgreSQL password | `litrater_password` |
| `POSTGRES_DB` | Database name | `litrater` |
| `POSTGRES_PORT` | PostgreSQL port | `5432` |
| `API_PORT` | API port | `5000` |
| `ASPNETCORE_ENVIRONMENT` | .NET environment | `Development` |
| `JWT_SECRET_KEY` | JWT signing key | `your-super-secret-key-change-this-in-production` |

### For Demo/Production-like Local Testing

Change the environment in your `.env` file:
```bash
ASPNETCORE_ENVIRONMENT=Production
POSTGRES_PASSWORD=your-secure-password
JWT_SECRET_KEY=your-super-secret-production-key
```

## Useful Commands

```bash
# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Rebuild and start
docker-compose -f docker/docker-compose.yml up -d --build

# Database operations
docker-compose -f docker/docker-compose.yml exec litrater-api dotnet ef database update
docker-compose -f docker/docker-compose.yml exec postgres psql -U litrater_user -d litrater

# Health check
curl http://localhost:5000/health
``` 