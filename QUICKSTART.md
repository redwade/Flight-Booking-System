# Quick Start Guide - Flight Booking System

Get the Flight Booking System up and running in minutes!

## Prerequisites

- ✅ .NET 9.0 SDK installed
- ✅ Docker Desktop installed and running
- ✅ Git (optional, for cloning)

## Option 1: Quick Start with Docker (Recommended)

### Step 1: Start All Services
```bash
cd FlightBookingSystem
docker-compose up -d
```

This single command will start:
- All 4 microservices (Booking, Flight, Payment, Notification)
- RabbitMQ message broker
- MongoDB database
- Redis cache
- PostgreSQL database

### Step 2: Verify Services are Running
```bash
docker-compose ps
```

You should see all services in "Up" status.

### Step 3: Access the APIs
Open your browser and navigate to:
- **Booking API**: http://localhost:5001/swagger
- **Flight API**: http://localhost:5002/swagger
- **Payment API**: http://localhost:5003/swagger
- **Notification API**: http://localhost:5004/swagger
- **RabbitMQ Management**: http://localhost:15672 (username: guest, password: guest)

### Step 4: Test the System

Use the Swagger UI or the provided `API_EXAMPLES.http` file to test the endpoints.

#### Quick Test Workflow:

1. **Create a Flight** (Flight API):
```json
POST http://localhost:5002/api/flights
{
  "flightNumber": "FL123",
  "airline": "Sky Airlines",
  "departureAirport": "JFK",
  "arrivalAirport": "LAX",
  "departureTime": "2025-10-15T10:00:00Z",
  "arrivalTime": "2025-10-15T15:00:00Z",
  "totalSeats": 150,
  "pricePerSeat": 250.00
}
```

2. **Create a Booking** (Booking API):
```json
POST http://localhost:5001/api/bookings
{
  "flightId": "<USE_FLIGHT_ID_FROM_STEP_1>",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "passengerName": "John Doe",
  "passengerEmail": "john.doe@example.com",
  "passengerPhone": "+1234567890",
  "numberOfSeats": 2,
  "totalAmount": 500.00
}
```

3. **Process Payment** (Payment API):
```json
POST http://localhost:5003/api/payments
{
  "bookingId": "<USE_BOOKING_ID_FROM_STEP_2>",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "amount": 500.00,
  "currency": "USD",
  "paymentMethod": "CreditCard"
}
```

4. **Check Notifications** (Notification API):
```
GET http://localhost:5004/api/notifications/user/3fa85f64-5717-4562-b3fc-2c963f66afa7
```

### Step 5: Monitor Messages in RabbitMQ
1. Go to http://localhost:15672
2. Login with guest/guest
3. Navigate to "Queues" tab
4. You'll see messages being processed between services

### Step 6: Stop All Services
```bash
docker-compose down
```

To remove volumes as well:
```bash
docker-compose down -v
```

---

## Option 2: Local Development Setup

### Step 1: Start Infrastructure Only
```bash
docker-compose up -d rabbitmq mongodb redis postgres
```

### Step 2: Build the Solution
```bash
# Make setup script executable (Unix/Mac)
chmod +x setup.sh
./setup.sh

# Or manually
dotnet restore
dotnet build
```

### Step 3: Run Each Microservice

Open 4 separate terminal windows:

**Terminal 1 - Booking API:**
```bash
cd src/Services/Booking/Booking.API/Booking.API
dotnet run
```

**Terminal 2 - Flight API:**
```bash
cd src/Services/Flight/Flight.API/Flight.API
dotnet run
```

**Terminal 3 - Payment API:**
```bash
cd src/Services/Payment/Payment.API/Payment.API
dotnet run
```

**Terminal 4 - Notification API:**
```bash
cd src/Services/Notification/Notification.API/Notification.API
dotnet run
```

### Step 4: Run Tests
```bash
dotnet test
```

---

## Troubleshooting

### Issue: Port Already in Use
**Solution**: Stop the conflicting service or change ports in docker-compose.yml

### Issue: Docker Services Won't Start
**Solution**: 
```bash
docker-compose down -v
docker-compose up -d
```

### Issue: Database Connection Errors
**Solution**: Wait a few seconds for databases to fully initialize, then restart the API services

### Issue: RabbitMQ Connection Failed
**Solution**: Ensure RabbitMQ is fully started (check with `docker-compose logs rabbitmq`)

---

## Next Steps

1. **Explore the APIs**: Use Swagger UI to test all endpoints
2. **Review Architecture**: Read `ARCHITECTURE.md` for detailed design information
3. **Check Logs**: Use `docker-compose logs -f <service-name>` to view logs
4. **Customize**: Modify `appsettings.json` files for each service
5. **Extend**: Add new features following the existing patterns

---

## Useful Commands

### Docker Commands
```bash
# View logs for all services
docker-compose logs -f

# View logs for specific service
docker-compose logs -f booking-api

# Restart a specific service
docker-compose restart booking-api

# View running containers
docker ps

# Stop all services
docker-compose down
```

### .NET Commands
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run specific project
dotnet run --project <path-to-csproj>

# Watch mode (auto-reload)
dotnet watch run
```

### Database Commands

**MongoDB:**
```bash
docker exec -it flightbooking-mongodb mongosh
```

**Redis:**
```bash
docker exec -it flightbooking-redis redis-cli
```

**PostgreSQL:**
```bash
docker exec -it flightbooking-postgres psql -U postgres -d PaymentDB
```

---

## API Endpoints Summary

| Service | Port | Swagger URL | Health Check |
|---------|------|-------------|--------------|
| Booking | 5001 | http://localhost:5001/swagger | http://localhost:5001/api/bookings/health |
| Flight | 5002 | http://localhost:5002/swagger | http://localhost:5002/api/flights/health |
| Payment | 5003 | http://localhost:5003/swagger | http://localhost:5003/api/payments/health |
| Notification | 5004 | http://localhost:5004/swagger | http://localhost:5004/api/notifications/health |

---

## Support

For issues or questions:
1. Check the main `README.md` for detailed documentation
2. Review `ARCHITECTURE.md` for design details
3. Examine logs using `docker-compose logs`
4. Create an issue in the repository

Happy coding! 🚀
