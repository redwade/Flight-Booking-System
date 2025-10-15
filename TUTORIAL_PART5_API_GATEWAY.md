# Tutorial Part 5: API Gateway with Ocelot

Welcome to Part 5 of the Flight Booking System tutorial! In this part, we'll add an API Gateway using Ocelot to provide a single entry point for all microservices.

## Table of Contents
1. [Overview](#overview)
2. [What is an API Gateway?](#what-is-an-api-gateway)
3. [Setting Up Ocelot](#setting-up-ocelot)
4. [Configuring Routes](#configuring-routes)
5. [Adding Rate Limiting](#adding-rate-limiting)
6. [Implementing Caching](#implementing-caching)
7. [Testing the Gateway](#testing-the-gateway)
8. [Advanced Features](#advanced-features)

## Overview

By the end of this tutorial, you'll have:
- ✅ A working API Gateway using Ocelot
- ✅ Unified access to all microservices
- ✅ Rate limiting to prevent abuse
- ✅ Response caching for better performance
- ✅ Environment-specific configurations

**Estimated Time**: 1-2 hours  
**Difficulty**: Intermediate

## What is an API Gateway?

### The Problem

Without an API Gateway, clients must know about and connect to multiple services:

```
Mobile App ──┬──> Booking Service (http://booking-api:5001)
             ├──> Flight Service (http://flight-api:5002)
Web App ─────┼──> Payment Service (http://payment-api:5003)
             ├──> Notification Service (http://notification-api:5004)
Desktop App ─┴──> AI Service (http://ai-api:5005)
```

**Issues:**
- Clients need to track multiple URLs
- Hard to change service locations
- No centralized security
- Difficult to implement cross-cutting concerns

### The Solution: API Gateway

```
Mobile App ───┐
Web App ──────┼──> API Gateway (http://gateway:5000) ──┬──> Booking Service
Desktop App ──┘                                         ├──> Flight Service
                                                        ├──> Payment Service
                                                        ├──> Notification Service
                                                        └──> AI Service
```

**Benefits:**
- ✅ Single entry point
- ✅ Simplified client code
- ✅ Centralized authentication
- ✅ Rate limiting and caching
- ✅ Load balancing
- ✅ Service discovery

## Setting Up Ocelot

### Step 1: Create the Gateway Project

```bash
# Navigate to src directory
cd src

# Create ApiGateway directory
mkdir -p ApiGateway/ApiGateway

# Create the project
cd ApiGateway/ApiGateway
dotnet new web -n ApiGateway
```

### Step 2: Install Ocelot Package

```bash
# Install Ocelot
dotnet add package Ocelot --version 23.3.3

# Install Ocelot Cache Manager
dotnet add package Ocelot.Cache.CacheManager --version 23.3.3
```

**Why these packages?**
- `Ocelot`: Core API Gateway functionality
- `Ocelot.Cache.CacheManager`: Response caching capabilities

### Step 3: Update ApiGateway.csproj

Your project file should look like this:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Ocelot" Version="23.3.3" />
    <PackageReference Include="Ocelot.Cache.CacheManager" Version="23.3.3" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
  </ItemGroup>

</Project>
```

### Step 4: Configure Program.cs

Replace the contents of `Program.cs`:

```csharp
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

// Add Ocelot configuration file
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot services
builder.Services.AddOcelot()
    .AddCacheManager(x =>
    {
        x.WithDictionaryHandle(); // Use in-memory cache
    });

// Add CORS for web applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Use Ocelot middleware
await app.UseOcelot();

app.Run();
```

**Code Explanation:**

1. **AddJsonFile("ocelot.json")**: Loads Ocelot configuration
2. **AddOcelot()**: Registers Ocelot services
3. **AddCacheManager()**: Enables response caching
4. **AddCors()**: Allows cross-origin requests
5. **UseOcelot()**: Activates the gateway middleware

## Configuring Routes

### Step 5: Create ocelot.json

Create `ocelot.json` in the ApiGateway project:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/bookings",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/bookings",
      "UpstreamHttpMethod": [ "Get", "Post" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

**Understanding the Configuration:**

- **DownstreamPathTemplate**: The actual service endpoint
- **DownstreamScheme**: Protocol (http/https)
- **DownstreamHostAndPorts**: Where the service is located
- **UpstreamPathTemplate**: What clients call
- **UpstreamHttpMethod**: Allowed HTTP methods
- **BaseUrl**: Gateway's public URL

### Step 6: Add Routes for All Services

Let's add routes for all microservices. Update `ocelot.json`:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/bookings",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/bookings",
      "UpstreamHttpMethod": [ "Get", "Post" ]
    },
    {
      "DownstreamPathTemplate": "/api/bookings/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/bookings/{id}",
      "UpstreamHttpMethod": [ "Get", "Put", "Delete" ]
    },
    {
      "DownstreamPathTemplate": "/api/flights/search",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "flight-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/flights/search",
      "UpstreamHttpMethod": [ "Get" ]
    },
    {
      "DownstreamPathTemplate": "/api/ai/chat",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "ai-api",
          "Port": 8080
        }
      ],
      "UpstreamPathTemplate": "/api/ai/chat",
      "UpstreamHttpMethod": [ "Post" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

**Pattern Matching:**
- `{id}` - Captures route parameters
- `/api/bookings` - Exact match
- Can use wildcards: `/api/{everything}`

## Adding Rate Limiting

### Step 7: Configure Rate Limits

Rate limiting prevents API abuse. Add to your routes:

```json
{
  "DownstreamPathTemplate": "/api/bookings",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "booking-api",
      "Port": 8080
    }
  ],
  "UpstreamPathTemplate": "/api/bookings",
  "UpstreamHttpMethod": [ "Get", "Post" ],
  "RateLimitOptions": {
    "ClientWhitelist": [],
    "EnableRateLimiting": true,
    "Period": "1m",
    "PeriodTimespan": 60,
    "Limit": 100
  }
}
```

**Rate Limit Parameters:**

- **EnableRateLimiting**: Turn on/off
- **Period**: Time window (1s, 1m, 1h, 1d)
- **PeriodTimespan**: Duration in seconds
- **Limit**: Max requests in period
- **ClientWhitelist**: IPs to exclude from limiting

**Example Limits:**
- Public endpoints: 10-50 requests/minute
- Authenticated users: 100-500 requests/minute
- Internal services: 1000+ requests/minute

### Step 8: Configure Global Rate Limit Settings

Add to `GlobalConfiguration`:

```json
{
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000",
    "RateLimitOptions": {
      "DisableRateLimitHeaders": false,
      "QuotaExceededMessage": "Rate limit exceeded. Please try again later.",
      "HttpStatusCode": 429
    }
  }
}
```

**When rate limit is exceeded:**
```http
HTTP/1.1 429 Too Many Requests
Content-Type: application/json

{
  "message": "Rate limit exceeded. Please try again later."
}
```

## Implementing Caching

### Step 9: Add Response Caching

Caching improves performance by storing responses:

```json
{
  "DownstreamPathTemplate": "/api/flights/search",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "flight-api",
      "Port": 8080
    }
  ],
  "UpstreamPathTemplate": "/api/flights/search",
  "UpstreamHttpMethod": [ "Get" ],
  "FileCacheOptions": {
    "TtlSeconds": 60,
    "Region": "flights"
  }
}
```

**Cache Parameters:**

- **TtlSeconds**: Time to live (how long to cache)
- **Region**: Cache namespace (for organization)

**Recommended TTLs:**
- Frequently changing data: 10-30 seconds
- Relatively stable data: 60-300 seconds
- Static data: 600+ seconds

**Example:**
```
First request:  Gateway → Service → Response (100ms)
Second request: Gateway → Cache → Response (5ms) ⚡
```

### Step 10: Cache Strategy by Service

Different services need different caching strategies:

```json
{
  "Routes": [
    {
      "UpstreamPathTemplate": "/api/flights/search",
      "FileCacheOptions": {
        "TtlSeconds": 60,
        "Region": "flights"
      }
    },
    {
      "UpstreamPathTemplate": "/api/bookings/{id}",
      "FileCacheOptions": {
        "TtlSeconds": 30,
        "Region": "bookings"
      }
    },
    {
      "UpstreamPathTemplate": "/api/notifications/user/{userId}",
      "FileCacheOptions": {
        "TtlSeconds": 15,
        "Region": "notifications"
      }
    },
    {
      "UpstreamPathTemplate": "/api/ai/chat/history",
      "FileCacheOptions": {
        "TtlSeconds": 10,
        "Region": "ai-history"
      }
    }
  ]
}
```

## Testing the Gateway

### Step 11: Create Development Configuration

For local development, create `ocelot.Development.json`:

```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/bookings",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 5001
        }
      ],
      "UpstreamPathTemplate": "/api/bookings",
      "UpstreamHttpMethod": [ "Get", "Post" ]
    },
    {
      "DownstreamPathTemplate": "/api/flights/search",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "localhost",
          "Port": 5002
        }
      ],
      "UpstreamPathTemplate": "/api/flights/search",
      "UpstreamHttpMethod": [ "Get" ]
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

**Why two configs?**
- **ocelot.json**: Docker (uses service names)
- **ocelot.Development.json**: Local (uses localhost)

### Step 12: Configure Launch Settings

Create `Properties/launchSettings.json`:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### Step 13: Run and Test

**Start the Gateway:**
```bash
cd src/ApiGateway/ApiGateway
dotnet run
```

**Test Health Checks:**
```bash
# Through gateway
curl http://localhost:5000/api/bookings/health
curl http://localhost:5000/api/flights/health
curl http://localhost:5000/api/ai/health
```

**Test Routing:**
```bash
# Create a booking through gateway
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "flightId": "flight456",
    "passengerName": "John Doe",
    "passengerEmail": "john@example.com",
    "numberOfSeats": 2
  }'
```

**Test Rate Limiting:**
```bash
# Send 101 requests quickly
for i in {1..101}; do
  curl http://localhost:5000/api/bookings/health
done

# Request 101 should return 429
```

**Test Caching:**
```bash
# First request - slow (hits backend)
time curl http://localhost:5000/api/flights/search?origin=NYC

# Second request - fast (from cache)
time curl http://localhost:5000/api/flights/search?origin=NYC
```

## Advanced Features

### Load Balancing

Distribute requests across multiple instances:

```json
{
  "DownstreamPathTemplate": "/api/bookings",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "booking-api-1",
      "Port": 8080
    },
    {
      "Host": "booking-api-2",
      "Port": 8080
    },
    {
      "Host": "booking-api-3",
      "Port": 8080
    }
  ],
  "LoadBalancerOptions": {
    "Type": "RoundRobin"
  },
  "UpstreamPathTemplate": "/api/bookings",
  "UpstreamHttpMethod": [ "Get", "Post" ]
}
```

**Load Balancer Types:**
- **RoundRobin**: Distribute evenly
- **LeastConnection**: Send to least busy
- **NoLoadBalancer**: Use first available

### Request Aggregation

Combine multiple service calls into one:

```json
{
  "Routes": [
    {
      "UpstreamPathTemplate": "/api/booking-details/{id}",
      "UpstreamHttpMethod": [ "Get" ],
      "DownstreamPathTemplate": "/api/bookings/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "booking-api",
          "Port": 8080
        }
      ],
      "Key": "booking"
    },
    {
      "UpstreamPathTemplate": "/api/booking-details/{id}",
      "UpstreamHttpMethod": [ "Get" ],
      "DownstreamPathTemplate": "/api/payments/booking/{id}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        {
          "Host": "payment-api",
          "Port": 8080
        }
      ],
      "Key": "payment"
    }
  ],
  "Aggregates": [
    {
      "RouteKeys": [ "booking", "payment" ],
      "UpstreamPathTemplate": "/api/booking-details/{id}"
    }
  ]
}
```

**Result:**
```json
{
  "booking": { /* booking data */ },
  "payment": { /* payment data */ }
}
```

### Quality of Service (QoS)

Add retry and circuit breaker:

```json
{
  "DownstreamPathTemplate": "/api/bookings",
  "QoSOptions": {
    "ExceptionsAllowedBeforeBreaking": 3,
    "DurationOfBreak": 1000,
    "TimeoutValue": 5000
  }
}
```

**Parameters:**
- **ExceptionsAllowedBeforeBreaking**: Failures before circuit opens
- **DurationOfBreak**: How long circuit stays open (ms)
- **TimeoutValue**: Request timeout (ms)

### Request Transformation

Modify requests before forwarding:

```json
{
  "DownstreamPathTemplate": "/api/v2/bookings",
  "UpstreamPathTemplate": "/api/bookings",
  "UpstreamHeaderTransform": {
    "X-Gateway-Version": "1.0",
    "X-Forwarded-For": "{RemoteIpAddress}"
  }
}
```

## Docker Integration

### Step 14: Create Dockerfile

Create `src/ApiGateway/Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/ApiGateway/ApiGateway/ApiGateway.csproj", "src/ApiGateway/ApiGateway/"]
RUN dotnet restore "src/ApiGateway/ApiGateway/ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/ApiGateway/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
```

### Step 15: Update docker-compose.yml

Add gateway to `docker-compose.yml`:

```yaml
services:
  api-gateway:
    build:
      context: .
      dockerfile: src/ApiGateway/Dockerfile
    container_name: api-gateway
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    depends_on:
      - booking-api
      - flight-api
      - payment-api
      - notification-api
      - ai-api
    networks:
      - flightbooking-network
```

### Step 16: Test with Docker

```bash
# Build and start
docker-compose up -d

# Test gateway
curl http://localhost:5000/api/bookings/health

# View logs
docker logs api-gateway
```

## Best Practices

### 1. Security

```json
{
  "Routes": [
    {
      "UpstreamPathTemplate": "/api/admin/{everything}",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": [ "admin" ]
      }
    }
  ]
}
```

### 2. Monitoring

Add logging:

```csharp
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});
```

### 3. Health Checks

Implement gateway health check:

```csharp
builder.Services.AddHealthChecks();

app.MapHealthChecks("/health");
```

### 4. Versioning

Support API versions:

```json
{
  "DownstreamPathTemplate": "/api/v1/bookings",
  "UpstreamPathTemplate": "/api/bookings",
  "Priority": 1
},
{
  "DownstreamPathTemplate": "/api/v2/bookings",
  "UpstreamPathTemplate": "/api/v2/bookings",
  "Priority": 0
}
```

## Troubleshooting

### Issue 1: Service Unavailable (503)

**Problem**: Gateway can't reach service

**Solutions:**
```bash
# Check if service is running
docker ps | grep booking-api

# Check network
docker network inspect flightbooking-network

# Verify service name in ocelot.json
```

### Issue 2: Configuration Not Loading

**Problem**: Changes not applied

**Solutions:**
```bash
# Restart gateway
docker restart api-gateway

# Check JSON syntax
cat ocelot.json | jq .

# Verify file is copied to container
docker exec api-gateway ls -la
```

### Issue 3: Rate Limit Not Working

**Problem**: Requests not being limited

**Solutions:**
- Check `EnableRateLimiting: true`
- Verify `Period` and `Limit` values
- Ensure global config is set
- Check logs for errors

### Issue 4: Cache Not Working

**Problem**: Every request hits backend

**Solutions:**
- Verify `FileCacheOptions` is set
- Check TTL is > 0
- Ensure HTTP method is GET
- Clear cache: restart gateway

## Summary

In this tutorial, you learned:

✅ What an API Gateway is and why it's useful  
✅ How to set up Ocelot in .NET 9.0  
✅ Configuring routes for multiple services  
✅ Implementing rate limiting  
✅ Adding response caching  
✅ Environment-specific configurations  
✅ Docker integration  
✅ Advanced features (load balancing, aggregation)  
✅ Best practices and troubleshooting  

### What You Built

- **API Gateway** on port 5000
- **18 routes** covering all microservices
- **Rate limiting** to protect services
- **Response caching** for performance
- **Docker support** for deployment

### Architecture Achievement

```
Before:
Client → 5 different services

After:
Client → API Gateway → 5 services
```

### Next Steps

1. **Add Authentication**: Implement JWT validation
2. **Add Logging**: Centralized logging with Serilog
3. **Add Metrics**: Prometheus/Grafana integration
4. **Add Tracing**: Distributed tracing
5. **Production Deploy**: HTTPS, load balancing, monitoring

## Resources

- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [Rate Limiting Guide](https://cloud.google.com/architecture/rate-limiting-strategies-techniques)
- [QUICKSTART_GATEWAY.md](./QUICKSTART_GATEWAY.md) - Quick reference
- [API_GATEWAY_README.md](./API_GATEWAY_README.md) - Complete guide

---

**Previous Tutorial**: [Part 4: AI Service with Gemma Integration](./TUTORIAL_PART4_AI_SERVICE.md)

**Congratulations!** You now have a complete microservices system with an API Gateway! 🎉🚀
