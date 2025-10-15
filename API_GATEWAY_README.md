# API Gateway - Ocelot

The API Gateway provides a single entry point for all microservices in the Flight Booking System using Ocelot.

## 🎯 What is an API Gateway?

An API Gateway is a server that acts as an API front-end, receiving API requests, enforcing throttling and security policies, passing requests to the back-end service, and then passing the response back to the requester.

### Benefits

- **Single Entry Point**: Clients interact with one endpoint instead of multiple microservices
- **Simplified Client Code**: No need to track multiple service URLs
- **Cross-Cutting Concerns**: Authentication, rate limiting, caching in one place
- **Load Balancing**: Distribute requests across multiple instances
- **Service Discovery**: Abstract service locations from clients
- **Protocol Translation**: Convert between different protocols

## 🏗️ Architecture

```
Client Application
       ↓
API Gateway (Port 5000)
       ↓
   ┌───┴───┬───────┬──────────┬────────┐
   ↓       ↓       ↓          ↓        ↓
Booking  Flight Payment  Notification  AI
Service  Service Service   Service   Service
(5001)   (5002)  (5003)    (5004)   (5005)
```

## 📡 Endpoints

All requests go through the API Gateway at `http://localhost:5000`

### Booking Service
- `GET/POST /api/bookings` - List/Create bookings
- `GET/PUT/DELETE /api/bookings/{id}` - Get/Update/Delete booking
- `GET /api/bookings/health` - Health check

### Flight Service
- `GET/POST /api/flights` - List/Create flights
- `GET /api/flights/search` - Search flights
- `GET /api/flights/health` - Health check

### Payment Service
- `GET/POST /api/payments` - List/Create payments
- `GET /api/payments/booking/{bookingId}` - Get payments by booking
- `GET /api/payments/health` - Health check

### Notification Service
- `GET /api/notifications/user/{userId}` - Get user notifications
- `GET /api/notifications/booking/{bookingId}` - Get booking notifications
- `GET /api/notifications/pending` - Get pending notifications
- `GET /api/notifications/health` - Health check

### AI Service
- `POST /api/ai/chat` - Chat with AI assistant
- `POST /api/ai/recommendations` - Get flight recommendations
- `GET /api/ai/chat/history` - Get chat history
- `GET /api/ai/health` - Health check

## 🚀 Getting Started

### Running Locally

1. **Start all backend services** (or use Docker):
```bash
# Start infrastructure
docker-compose up -d rabbitmq mongodb redis postgres ollama

# Start each service in separate terminals
cd src/Services/Booking/Booking.API/Booking.API && dotnet run
cd src/Services/Flight/Flight.API/Flight.API && dotnet run
cd src/Services/Payment/Payment.API/Payment.API && dotnet run
cd src/Services/Notification/Notification.API/Notification.API && dotnet run
cd src/Services/AI/AI.API/AI.API && dotnet run
```

2. **Start the API Gateway**:
```bash
cd src/ApiGateway/ApiGateway
dotnet run
```

3. **Access via Gateway**:
```bash
# Instead of http://localhost:5001/api/bookings
# Use:
curl http://localhost:5000/api/bookings
```

### Running with Docker

```bash
# Start everything including gateway
docker-compose up -d

# Gateway is available at http://localhost:5000
```

## 🔧 Configuration

### ocelot.json (Docker/Production)

Routes requests to Docker service names:
```json
{
  "DownstreamHostAndPorts": [
    {
      "Host": "booking-api",
      "Port": 8080
    }
  ]
}
```

### ocelot.Development.json (Local Development)

Routes requests to localhost ports:
```json
{
  "DownstreamHostAndPorts": [
    {
      "Host": "localhost",
      "Port": 5001
    }
  ]
}
```

## ⚡ Features

### 1. Rate Limiting

Protects services from abuse:

```json
"RateLimitOptions": {
  "ClientWhitelist": [],
  "EnableRateLimiting": true,
  "Period": "1m",
  "PeriodTimespan": 60,
  "Limit": 100
}
```

**Configured Limits:**
- Booking Service: 100 requests/minute
- Flight Service: 100 requests/minute
- Payment Service: 50 requests/minute
- AI Service: 20 requests/minute

### 2. Response Caching

Improves performance by caching responses:

```json
"FileCacheOptions": {
  "TtlSeconds": 60,
  "Region": "flights"
}
```

**Cache Configuration:**
- Bookings: 30 seconds
- Flights: 60 seconds
- Notifications: 15 seconds
- AI History: 10 seconds

### 3. CORS Support

Allows cross-origin requests from web applications.

### 4. Load Balancing

Can distribute requests across multiple service instances (configure multiple hosts).

## 📝 Usage Examples

### Before (Direct Service Access)
```bash
# Different URLs for each service
curl http://localhost:5001/api/bookings
curl http://localhost:5002/api/flights
curl http://localhost:5003/api/payments
curl http://localhost:5004/api/notifications
curl http://localhost:5005/api/ai/chat
```

### After (Through Gateway)
```bash
# Single URL for all services
curl http://localhost:5000/api/bookings
curl http://localhost:5000/api/flights
curl http://localhost:5000/api/payments
curl http://localhost:5000/api/notifications
curl http://localhost:5000/api/ai/chat
```

### Example: Create Booking Through Gateway

```bash
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

### Example: Search Flights Through Gateway

```bash
curl "http://localhost:5000/api/flights/search?origin=NYC&destination=LON&date=2025-11-15"
```

### Example: AI Chat Through Gateway

```bash
curl -X POST http://localhost:5000/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "message": "I want to book a flight to Paris"
  }'
```

## 🔒 Security Features

### Rate Limiting
- Prevents API abuse
- Different limits per service
- Returns 429 status when exceeded

### CORS
- Configured for cross-origin requests
- Can be restricted to specific domains in production

### Future Enhancements
- [ ] JWT Authentication
- [ ] API Key validation
- [ ] Request/Response logging
- [ ] Circuit breaker pattern
- [ ] Request transformation
- [ ] Response aggregation

## 🎯 Advanced Configuration

### Adding a New Route

Edit `ocelot.json`:

```json
{
  "DownstreamPathTemplate": "/api/newservice/{id}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "newservice-api",
      "Port": 8080
    }
  ],
  "UpstreamPathTemplate": "/api/newservice/{id}",
  "UpstreamHttpMethod": [ "Get", "Post" ]
}
```

### Load Balancing Multiple Instances

```json
{
  "DownstreamHostAndPorts": [
    {
      "Host": "booking-api-1",
      "Port": 8080
    },
    {
      "Host": "booking-api-2",
      "Port": 8080
    }
  ],
  "LoadBalancerOptions": {
    "Type": "RoundRobin"
  }
}
```

### Request Aggregation

Combine multiple service calls into one:

```json
{
  "UpstreamPathTemplate": "/api/booking-details/{id}",
  "UpstreamHttpMethod": [ "Get" ],
  "Aggregates": [
    "booking",
    "payment",
    "notification"
  ]
}
```

## 📊 Monitoring

### Health Checks

Check all services through gateway:

```bash
curl http://localhost:5000/api/bookings/health
curl http://localhost:5000/api/flights/health
curl http://localhost:5000/api/payments/health
curl http://localhost:5000/api/notifications/health
curl http://localhost:5000/api/ai/health
```

### Logs

View gateway logs:
```bash
# Docker
docker logs api-gateway

# Local
# Check console output
```

## 🐛 Troubleshooting

### "Service Unavailable" Error

**Problem**: Gateway can't reach backend service

**Solutions**:
```bash
# Check if services are running
docker ps

# Check service health
curl http://localhost:5001/api/bookings/health

# Verify network connectivity
docker network inspect flightbooking-network
```

### Rate Limit Exceeded

**Problem**: Too many requests

**Response**:
```json
{
  "message": "Rate limit exceeded. Please try again later."
}
```

**Solution**: Wait for the rate limit window to reset or increase limits in `ocelot.json`

### Cache Issues

**Problem**: Getting stale data

**Solution**:
```bash
# Restart gateway to clear cache
docker restart api-gateway

# Or reduce TTL in ocelot.json
```

### Configuration Not Loading

**Problem**: Changes to ocelot.json not applied

**Solution**:
```bash
# Restart the gateway
docker restart api-gateway

# Or for local development
# Stop and restart dotnet run
```

## 🔄 Development Workflow

### Local Development

1. Run services individually on their ports (5001-5005)
2. Gateway uses `ocelot.Development.json` (localhost routing)
3. Test through gateway at port 5000

### Docker Development

1. Run all services with `docker-compose up`
2. Gateway uses `ocelot.json` (Docker service names)
3. Test through gateway at port 5000

### Production

1. Deploy all services
2. Configure `ocelot.json` with production URLs
3. Add authentication and security
4. Enable HTTPS
5. Configure proper rate limits

## 📚 Best Practices

### 1. Use Appropriate Cache TTLs
- Frequently changing data: 10-30 seconds
- Relatively stable data: 60-300 seconds
- Static data: 600+ seconds

### 2. Set Reasonable Rate Limits
- Public endpoints: Lower limits (10-50/min)
- Authenticated users: Medium limits (100-500/min)
- Internal services: Higher limits (1000+/min)

### 3. Monitor Gateway Performance
- Track response times
- Monitor error rates
- Watch cache hit ratios
- Alert on rate limit violations

### 4. Version Your APIs
```json
{
  "UpstreamPathTemplate": "/api/v1/bookings",
  "DownstreamPathTemplate": "/api/bookings"
}
```

### 5. Use Health Checks
- Implement health endpoints in all services
- Monitor through gateway
- Set up automated alerts

## 🎓 Learning Resources

- [Ocelot Documentation](https://ocelot.readthedocs.io/)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [Rate Limiting Best Practices](https://cloud.google.com/architecture/rate-limiting-strategies-techniques)

## 📈 Performance Tips

1. **Enable Caching**: Reduce backend load
2. **Use Compression**: Reduce bandwidth
3. **Implement Circuit Breaker**: Prevent cascade failures
4. **Monitor Metrics**: Track and optimize
5. **Scale Horizontally**: Run multiple gateway instances

## 🚀 Next Steps

1. **Add Authentication**: Implement JWT validation
2. **Add Logging**: Centralized logging with Serilog
3. **Add Metrics**: Prometheus/Grafana integration
4. **Add Tracing**: Distributed tracing with OpenTelemetry
5. **Add Circuit Breaker**: Polly integration
6. **Add Service Discovery**: Consul integration

---

**Gateway URL**: http://localhost:5000  
**Configuration**: `src/ApiGateway/ApiGateway/ocelot.json`  
**Documentation**: This file

Happy routing! 🚀
