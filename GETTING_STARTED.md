# Getting Started - Flight Booking System

## 📋 Prerequisites Checklist

Before you begin, ensure you have the following installed:

- [ ] **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- [ ] **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- [ ] **Git** (optional) - [Download](https://git-scm.com/downloads)
- [ ] **IDE** - Visual Studio 2022, VS Code, or JetBrains Rider

### Verify Installations

```bash
# Check .NET version
dotnet --version
# Should show 9.0.x

# Check Docker
docker --version
docker-compose --version
```

## 🚀 Step-by-Step Setup

### Step 1: Navigate to Project Directory
```bash
cd /Users/tominjose/CascadeProjects/FlightBookingSystem
```

### Step 2: Add NuGet Packages and References
```bash
chmod +x add-packages.sh
./add-packages.sh
```

**Expected Output**: All packages should install successfully without errors.

### Step 3: Restore and Build
```bash
# Restore NuGet packages
dotnet restore FlightBookingSystem.sln

# Build the solution
dotnet build FlightBookingSystem.sln
```

**Expected Output**: Build succeeded with 0 errors.

### Step 4: Run Tests (Optional)
```bash
dotnet test FlightBookingSystem.sln
```

**Expected Output**: All tests should pass.

### Step 5: Start Infrastructure Services
```bash
# Start RabbitMQ, MongoDB, Redis, and PostgreSQL
docker-compose up -d rabbitmq mongodb redis postgres
```

**Verify Services**:
```bash
docker-compose ps
```

All services should show "Up" status.

### Step 6: Run the Microservices

#### Option A: Run with Docker (Recommended)
```bash
docker-compose up -d
```

#### Option B: Run Locally (Development)

Open 4 terminal windows and run:

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

### Step 7: Verify APIs are Running

Open your browser and check:

- ✅ Booking API: http://localhost:5001/swagger
- ✅ Flight API: http://localhost:5002/swagger
- ✅ Payment API: http://localhost:5003/swagger
- ✅ Notification API: http://localhost:5004/swagger
- ✅ RabbitMQ Management: http://localhost:15672 (guest/guest)

### Step 8: Test the System

#### Quick Health Check
```bash
curl http://localhost:5001/api/bookings/health
curl http://localhost:5002/api/flights/health
curl http://localhost:5003/api/payments/health
curl http://localhost:5004/api/notifications/health
```

All should return `{"status":"healthy",...}`

#### Complete Workflow Test

**1. Create a Flight:**
```bash
curl -X POST http://localhost:5002/api/flights \
  -H "Content-Type: application/json" \
  -d '{
    "flightNumber": "FL123",
    "airline": "Sky Airlines",
    "departureAirport": "JFK",
    "arrivalAirport": "LAX",
    "departureTime": "2025-10-15T10:00:00Z",
    "arrivalTime": "2025-10-15T15:00:00Z",
    "totalSeats": 150,
    "pricePerSeat": 250.00
  }'
```

**2. Search Flights:**
```bash
curl "http://localhost:5002/api/flights/search?departureAirport=JFK&arrivalAirport=LAX&departureDate=2025-10-15"
```

**3. Create a Booking** (use FlightId from step 1):
```bash
curl -X POST http://localhost:5001/api/bookings \
  -H "Content-Type: application/json" \
  -d '{
    "flightId": "YOUR_FLIGHT_ID_HERE",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "passengerName": "John Doe",
    "passengerEmail": "john.doe@example.com",
    "passengerPhone": "+1234567890",
    "numberOfSeats": 2,
    "totalAmount": 500.00
  }'
```

**4. Process Payment** (use BookingId from step 3):
```bash
curl -X POST http://localhost:5003/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": "YOUR_BOOKING_ID_HERE",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "amount": 500.00,
    "currency": "USD",
    "paymentMethod": "CreditCard"
  }'
```

**5. Check Notifications:**
```bash
curl "http://localhost:5004/api/notifications/user/3fa85f64-5717-4562-b3fc-2c963f66afa7"
```

## 🎯 What to Explore Next

### 1. Swagger UI
- Navigate to each API's Swagger page
- Try out different endpoints
- View request/response schemas

### 2. RabbitMQ Management Console
- Go to http://localhost:15672
- Login: guest/guest
- Explore queues and exchanges
- Watch messages flow between services

### 3. Database Inspection

**MongoDB (Flight Service):**
```bash
docker exec -it flightbooking-mongodb mongosh
use FlightDB
db.flights.find()
```

**Redis (Booking Service):**
```bash
docker exec -it flightbooking-redis redis-cli
KEYS booking:*
GET booking:YOUR_BOOKING_ID
```

**PostgreSQL (Payment Service):**
```bash
docker exec -it flightbooking-postgres psql -U postgres -d PaymentDB
\dt
SELECT * FROM "Payments";
```

### 4. View Logs

**Docker Logs:**
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f booking-api
docker-compose logs -f flight-api
docker-compose logs -f payment-api
docker-compose logs -f notification-api
```

**Local Development Logs:**
Check the terminal windows where services are running.

## 🔧 Development Workflow

### Making Changes

1. **Modify Code**: Edit files in your IDE
2. **Build**: `dotnet build`
3. **Test**: `dotnet test`
4. **Run**: Restart the affected service

### Hot Reload (Development)
```bash
cd src/Services/Booking/Booking.API/Booking.API
dotnet watch run
```

### Adding New Features

1. **Add Entity** in Core layer
2. **Add Repository Interface** in Core layer
3. **Implement Repository** in Infrastructure layer
4. **Create Command/Query** in Application layer
5. **Create Handler** in Application layer
6. **Add Controller Endpoint** in API layer
7. **Write Tests** in Test project

## 📚 Learning Path

### Beginner
1. ✅ Run the system and test APIs
2. ✅ Explore Swagger documentation
3. ✅ Watch RabbitMQ message flow
4. ✅ Review code structure

### Intermediate
1. ✅ Modify existing endpoints
2. ✅ Add new API endpoints
3. ✅ Create new message events
4. ✅ Write unit tests

### Advanced
1. ✅ Add new microservice
2. ✅ Implement saga pattern
3. ✅ Add API Gateway
4. ✅ Deploy to Kubernetes

## 🐛 Troubleshooting

### Problem: Build Errors

**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Problem: Port Already in Use

**Solution:**
```bash
# Find and kill process
lsof -ti:5001 | xargs kill -9

# Or change port in appsettings.json
```

### Problem: Docker Services Won't Start

**Solution:**
```bash
docker-compose down -v
docker system prune -a
docker-compose up -d
```

### Problem: Database Connection Errors

**Solution:**
1. Wait 10-15 seconds for databases to initialize
2. Check Docker logs: `docker-compose logs mongodb`
3. Verify connection strings in appsettings.json

### Problem: RabbitMQ Connection Failed

**Solution:**
```bash
# Check RabbitMQ status
docker-compose logs rabbitmq

# Restart RabbitMQ
docker-compose restart rabbitmq
```

### Problem: Tests Failing

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
dotnet test --no-build
```

## 📖 Additional Resources

- **README.md** - Complete project documentation
- **ARCHITECTURE.md** - Detailed architecture guide
- **QUICKSTART.md** - Quick reference guide
- **API_EXAMPLES.http** - HTTP request examples
- **PROJECT_SUMMARY.md** - Project overview

## 🎓 Key Concepts to Understand

1. **Microservices**: Independent, deployable services
2. **Clean Architecture**: Separation of concerns in layers
3. **CQRS**: Separate read and write operations
4. **Event-Driven**: Services communicate via events
5. **Polyglot Persistence**: Different databases for different needs
6. **Message Broker**: Asynchronous communication via RabbitMQ
7. **Docker**: Containerization for consistency
8. **REST API**: HTTP-based service communication

## ✅ Success Checklist

After completing setup, you should be able to:

- [ ] Build the solution without errors
- [ ] Run all tests successfully
- [ ] Start all services with Docker
- [ ] Access all Swagger UIs
- [ ] Create a flight via API
- [ ] Search for flights
- [ ] Create a booking
- [ ] Process a payment
- [ ] View notifications
- [ ] See messages in RabbitMQ
- [ ] Query databases directly

## 🎉 You're Ready!

If all the above works, congratulations! You now have a fully functional microservices system running.

**Next Steps:**
1. Explore the codebase
2. Try modifying features
3. Add your own endpoints
4. Experiment with the architecture

**Need Help?**
- Check documentation files
- Review code comments
- Examine test files for examples
- Create issues for bugs

---

**Happy Coding! 🚀**
