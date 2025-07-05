# Deployment Guide

Simple deployment guide for portfolio demonstration.

## Local Development

```bash
# Start services
docker-compose -f docker/docker-compose.yml up -d

# Access at http://localhost:5000
```

## Cloud Deployment

### Option 1: Railway (Recommended for Portfolio)

1. **Fork/Clone your repository**
2. **Connect to Railway:**
   - Go to [railway.app](https://railway.app)
   - Connect your GitHub repository
   - Railway will auto-detect Docker setup

3. **Set Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   POSTGRES_PASSWORD=your-secure-password
   JWT_SECRET_KEY=your-super-secret-production-key
   ```

4. **Deploy:**
   - Railway will automatically build and deploy
   - Get your live URL from Railway dashboard

### Option 2: Render

1. **Connect repository to Render**
2. **Create Web Service:**
   - Build Command: `docker build -t litrater .`
   - Start Command: `docker-compose -f docker/docker-compose.yml up`
3. **Add PostgreSQL service**
4. **Set environment variables**

### Option 3: DigitalOcean App Platform

1. **Connect your repository**
2. **Choose Docker deployment**
3. **Add PostgreSQL database**
4. **Configure environment variables**

## Environment Variables for Production

```bash
ASPNETCORE_ENVIRONMENT=Production
POSTGRES_PASSWORD=your-secure-password
JWT_SECRET_KEY=your-super-secret-production-key
POSTGRES_USER=litrater_user
POSTGRES_DB=litrater
```

## Health Check

Your deployed application will have a health check endpoint:
```
https://your-app-url/health
```

## Database Migrations

Migrations run automatically in development. For production:
```bash
docker-compose -f docker/docker-compose.yml exec litrater-api dotnet ef database update
```

COMPOSE_PROJECT_NAME=litrater 