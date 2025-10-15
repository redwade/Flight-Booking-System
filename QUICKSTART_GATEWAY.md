# Quick Start - API Gateway

Get the API Gateway running in 2 minutes!

## What is the API Gateway?

The API Gateway is your **single entry point** to all microservices. Instead of calling 5 different URLs, you call one:

```
Before: http://localhost:5001, 5002, 5003, 5004, 5005
After:  http://localhost:5000 (for everything!)
```

## Quick Start

### Option 1: Docker (Easiest)

```bash
# Start everything
docker-compose up -d

# That's it! Gateway is at http://localhost:5000
```

### Option 2: Local Development

```bash
# 1. Start all backend services (or use docker for infrastructure)
docker-compose up -d rabbitmq mongodb redis postgres ollama

# 2. Start microservices in separate terminals
cd src/Services/Booking/Booking.API/Booking.API && dotnet run
cd src/Services/Flight/Flight.API/Flight.API && dotnet run
cd src/Services/Payment/Payment.API/Payment.API && dotnet run
cd src/Services/Notification/Notification.API/Notification.API && dotnet run
cd src/Services/AI/AI.API/AI.API && dotnet run

# 3. Start the gateway
cd src/ApiGateway/ApiGateway && dotnet run
```

## Test It!

### Health Check All Services

```bash
curl http://localhost:5000/api/bookings/health
curl http://localhost:5000/api/flights/health
curl http://localhost:5000/api/payments/health
curl http://localhost:5000/api/notifications/health
curl http://localhost:5000/api/ai/health
```

### Create a Booking Through Gateway

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

### Search Flights Through Gateway

```bash
curl "http://localhost:5000/api/flights/search?origin=NYC&destination=LON"
```

### Chat with AI Through Gateway

```bash
curl -X POST http://localhost:5000/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "message": "I want to book a flight to Paris"
  }'
```

## Features

### ✅ Rate Limiting
- Booking: 100 requests/minute
- Flight: 100 requests/minute
- Payment: 50 requests/minute
- AI: 20 requests/minute

Try exceeding the limit and you'll get:
```json
{
  "message": "Rate limit exceeded. Please try again later."
}
```

### ✅ Response Caching
- Flights: Cached for 60 seconds
- Bookings: Cached for 30 seconds
- Notifications: Cached for 15 seconds
- AI History: Cached for 10 seconds

Faster responses for repeated queries!

### ✅ CORS Support
Web applications can call the API from any origin.

## Architecture

```
Your App
    ↓
API Gateway (Port 5000)
    ↓
┌───┴────┬────────┬─────────┬─────────┬────────┐
↓        ↓        ↓         ↓         ↓        ↓
Booking Flight Payment Notification  AI    RabbitMQ
(5001)  (5002)  (5003)    (5004)   (5005)
```

## Port Reference

- **5000** - API Gateway ⭐ **Use this!**
- 5001 - Booking Service (direct)
- 5002 - Flight Service (direct)
- 5003 - Payment Service (direct)
- 5004 - Notification Service (direct)
- 5005 - AI Service (direct)

## Configuration Files

### For Docker
`src/ApiGateway/ApiGateway/ocelot.json`
- Routes to Docker service names (booking-api, flight-api, etc.)

### For Local Development
`src/ApiGateway/ApiGateway/ocelot.Development.json`
- Routes to localhost:5001, localhost:5002, etc.

## Common Issues

### "Service Unavailable"
**Problem**: Gateway can't reach a service

**Solution**:
```bash
# Check if services are running
docker ps

# Or for local development
# Make sure all services are started
```

### Rate Limit Hit
**Problem**: Too many requests

**Solution**: Wait 60 seconds or adjust limits in `ocelot.json`

### Wrong Configuration
**Problem**: Routes not working

**Solution**:
```bash
# Restart gateway
docker restart api-gateway

# Or for local
# Stop and restart dotnet run
```

## Testing with HTTP Files

Open `src/ApiGateway/ApiGateway/ApiGateway.http` in VS Code with REST Client extension:

1. Install "REST Client" extension
2. Open ApiGateway.http
3. Click "Send Request" above any request
4. See the response instantly!

## Next Steps

1. **Use the Gateway**: Always call `http://localhost:5000` instead of individual services
2. **Check Logs**: Monitor gateway logs for routing issues
3. **Customize**: Edit `ocelot.json` to adjust rate limits and cache TTLs
4. **Add Auth**: Implement JWT authentication (see API_GATEWAY_README.md)
5. **Monitor**: Track response times and error rates

## Benefits

✅ **Simplified Client Code**: One URL instead of five  
✅ **Rate Limiting**: Protect services from abuse  
✅ **Caching**: Faster responses  
✅ **Load Balancing**: Ready for multiple instances  
✅ **Security**: Single point for authentication  
✅ **Monitoring**: Centralized logging and metrics  

## Full Documentation

For advanced features, see:
- [API_GATEWAY_README.md](./API_GATEWAY_README.md) - Complete documentation
- [README.md](./README.md) - System overview

## Example Workflow

```bash
# 1. Start everything
docker-compose up -d

# 2. Create a flight
curl -X POST http://localhost:5000/api/flights \
  -H "Content-Type: application/json" \
  -d '{
    "flightNumber": "BA123",
    "origin": "New York",
    "destination": "London",
    "departureTime": "2025-11-15T10:00:00Z",
    "arrivalTime": "2025-11-15T22:00:00Z",
    "availableSeats": 150,
    "price": 599.99
  }'

# 3. Search for flights
curl "http://localhost:5000/api/flights/search?origin=New%20York&destination=London"

# 4. Get AI recommendations
curl -X POST http://localhost:5000/api/ai/recommendations \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "origin": "New York",
    "destination": "London",
    "departureDate": "2025-11-15",
    "preferredClass": "Economy",
    "maxBudget": 1000
  }'

# 5. Create a booking
curl -X POST http://localhost:5000/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "flightId": "<flight-id-from-step-2>",
    "passengerName": "John Doe",
    "passengerEmail": "john@example.com",
    "numberOfSeats": 1
  }'

# 6. Process payment
curl -X POST http://localhost:5000/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": "<booking-id-from-step-5>",
    "userId": "user123",
    "amount": 599.99,
    "currency": "USD",
    "paymentMethod": "CreditCard"
  }'

# 7. Check notifications
curl http://localhost:5000/api/notifications/user/user123
```

All through one gateway! 🚀

---

**Gateway URL**: http://localhost:5000  
**Status**: Ready to use  
**Documentation**: [API_GATEWAY_README.md](./API_GATEWAY_README.md)
