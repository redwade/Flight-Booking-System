# 🎓 Beginner's Tutorial Part 3 - API Layer & Testing

## Overview
This is Part 3 of the complete beginner's tutorial. This part covers:
- Creating API controllers
- Configuring Program.cs
- Setting up appsettings.json
- Running the system
- Testing with Swagger and HTTP requests

**Estimated Time**: 1-2 hours  
**Difficulty**: Beginner-Intermediate

---

## Step 10: Create API Layer - Booking Service (30 minutes)

### 10.1 Add Project References

```bash
cd src/Services/Booking/Booking.API/Booking.API
dotnet add reference ../../Booking.Core/Booking.Core/Booking.Core.csproj
dotnet add reference ../../Booking.Application/Booking.Application/Booking.Application.csproj
dotnet add reference ../../Booking.Infrastructure/Booking.Infrastructure/Booking.Infrastructure.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj
```

### 10.2 Create Controller

**File**: `src/Services/Booking/Booking.API/Booking.API/Controllers/BookingsController.cs`

Create a new folder `Controllers` and add:

```csharp
using Booking.Application.Commands;
using Booking.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<CreateBookingCommandResponse>> CreateBooking(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBookingById), new { id = result.BookingId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, "An error occurred while creating the booking");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetBookingByIdQueryResponse>> GetBookingById(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetBookingByIdQuery(id), cancellationToken);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            return StatusCode(500, "An error occurred while retrieving the booking");
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Booking API", timestamp = DateTime.UtcNow });
    }
}
```

**Understanding the controller**:
- **[ApiController]**: Enables API-specific features
- **[Route]**: Defines URL pattern (api/bookings)
- **[HttpPost]**: Handles POST requests
- **[HttpGet]**: Handles GET requests
- **IMediator**: Sends commands/queries to handlers
- **ActionResult<T>**: Return type for API responses

### 10.3 Update Program.cs

**File**: `src/Services/Booking/Booking.API/Booking.API/Program.cs`

Replace the entire content with:

```csharp
using Booking.Application;
using Booking.Infrastructure;
using BuildingBlocks.MassTransit.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure layers
builder.Services.AddBookingApplication();
builder.Services.AddBookingInfrastructure(builder.Configuration);

// Add MassTransit with RabbitMQ
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

**Understanding Program.cs**:
- **AddControllers()**: Enables MVC controllers
- **AddSwaggerGen()**: Adds Swagger documentation
- **AddBookingApplication()**: Registers MediatR handlers
- **AddBookingInfrastructure()**: Registers repositories
- **AddMassTransitWithRabbitMq()**: Configures message broker
- **MapControllers()**: Maps controller routes

### 10.4 Update appsettings.json

**File**: `src/Services/Booking/Booking.API/Booking.API/appsettings.json`

Replace content with:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

**Understanding configuration**:
- **ConnectionStrings**: Database connection info
- **RabbitMQ**: Message broker settings
- **Logging**: Log level configuration

---

## Step 11: Create Docker Compose (20 minutes)

### 11.1 Create docker-compose.yml

**File**: `docker-compose.yml` (in root directory)

```yaml
version: '3.8'

services:
  # RabbitMQ Message Broker
  rabbitmq:
    image: rabbitmq:3-management
    container_name: flightbooking-rabbitmq
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq
    networks:
      - flightbooking-network

  # MongoDB for Flight Service
  mongodb:
    image: mongo:latest
    container_name: flightbooking-mongodb
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: admin123
    volumes:
      - mongodb_data:/data/db
    networks:
      - flightbooking-network

  # Redis for Booking Service
  redis:
    image: redis:alpine
    container_name: flightbooking-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - flightbooking-network

  # PostgreSQL for Payment Service
  postgres:
    image: postgres:latest
    container_name: flightbooking-postgres
    ports:
      - "5432:5432"
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: PaymentDB
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - flightbooking-network

volumes:
  rabbitmq_data:
  mongodb_data:
  redis_data:
  postgres_data:

networks:
  flightbooking-network:
    driver: bridge
```

**Understanding Docker Compose**:
- **services**: Defines containers to run
- **ports**: Maps container ports to host
- **environment**: Sets environment variables
- **volumes**: Persists data between restarts
- **networks**: Allows containers to communicate

---

## Step 12: Build and Run (15 minutes)

### 12.1 Build the Solution

```bash
# From root directory
dotnet build FlightBookingSystem.sln
```

**Expected output**: Build succeeded with 0 errors

### 12.2 Start Infrastructure Services

```bash
docker-compose up -d rabbitmq mongodb redis postgres
```

**What this does**: Starts all database and message broker services in background

### 12.3 Verify Services Running

```bash
docker-compose ps
```

**Expected output**: All services should show "Up" status

### 12.4 Run Booking API

```bash
cd src/Services/Booking/Booking.API/Booking.API
dotnet run
```

**Expected output**:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
```

### 12.5 Access Swagger UI

Open browser and navigate to:
```
http://localhost:5001/swagger
```

You should see the Swagger documentation page with your API endpoints!

---

## Step 13: Testing Your API (20 minutes)

### 13.1 Test Health Endpoint

**Using browser**:
```
http://localhost:5001/api/bookings/health
```

**Expected response**:
```json
{
  "status": "healthy",
  "service": "Booking API",
  "timestamp": "2025-10-11T13:30:00Z"
}
```

### 13.2 Create a Booking (Using Swagger)

1. In Swagger UI, expand **POST /api/bookings**
2. Click **Try it out**
3. Enter this JSON in the request body:

```json
{
  "flightId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "passengerName": "John Doe",
  "passengerEmail": "john.doe@example.com",
  "passengerPhone": "+1234567890",
  "numberOfSeats": 2,
  "totalAmount": 500.00
}
```

4. Click **Execute**

**Expected response** (Status 201 Created):
```json
{
  "bookingId": "generated-guid-here",
  "bookingReference": "BK20251011130000001",
  "status": "Pending"
}
```

### 13.3 Get Booking by ID

1. Copy the `bookingId` from previous response
2. In Swagger, expand **GET /api/bookings/{id}**
3. Click **Try it out**
4. Paste the booking ID
5. Click **Execute**

**Expected response** (Status 200 OK):
```json
{
  "id": "your-booking-id",
  "flightId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "passengerName": "John Doe",
  "passengerEmail": "john.doe@example.com",
  "passengerPhone": "+1234567890",
  "numberOfSeats": 2,
  "totalAmount": 500.00,
  "status": "Pending",
  "bookingDate": "2025-10-11T13:30:00Z",
  "bookingReference": "BK20251011130000001"
}
```

### 13.4 Verify Data in Redis

```bash
# Connect to Redis container
docker exec -it flightbooking-redis redis-cli

# List all booking keys
KEYS booking:*

# Get a specific booking (replace with your booking ID)
GET booking:your-booking-id-here

# Exit Redis
exit
```

---

## Step 14: Create API Examples File (10 minutes)

### 14.1 Create HTTP Request File

**File**: `API_EXAMPLES.http` (in root directory)

```http
### Variables
@bookingApiUrl = http://localhost:5001

### Health Check
GET {{bookingApiUrl}}/api/bookings/health

### Create Booking
POST {{bookingApiUrl}}/api/bookings
Content-Type: application/json

{
  "flightId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "passengerName": "John Doe",
  "passengerEmail": "john.doe@example.com",
  "passengerPhone": "+1234567890",
  "numberOfSeats": 2,
  "totalAmount": 500.00
}

### Get Booking by ID (replace with actual ID)
GET {{bookingApiUrl}}/api/bookings/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

**How to use**:
- Install REST Client extension in VS Code
- Click "Send Request" above each request

---

## Step 15: Understanding the Complete Flow (10 minutes)

### Request Flow Diagram

```
1. User → HTTP Request
   ↓
2. Controller → Receives request
   ↓
3. MediatR → Routes to handler
   ↓
4. Handler → Business logic
   ↓
5. Repository → Database operation
   ↓
6. MassTransit → Publish event (optional)
   ↓
7. Handler → Returns response
   ↓
8. Controller → HTTP Response
   ↓
9. User ← JSON Response
```

### Example: Creating a Booking

1. **User sends POST request** to `/api/bookings`
2. **BookingsController** receives request
3. **MediatR** sends `CreateBookingCommand` to handler
4. **CreateBookingCommandHandler**:
   - Creates `BookingEntity`
   - Calls `_repository.CreateAsync()`
   - Publishes `BookingCreatedEvent` to RabbitMQ
   - Returns `CreateBookingCommandResponse`
5. **Controller** returns 201 Created with booking details
6. **User** receives JSON response

---

## Step 16: Common Issues & Solutions (Reference)

### Issue 1: Port Already in Use
**Error**: "Address already in use"

**Solution**:
```bash
# Find process using port 5001
lsof -ti:5001

# Kill the process
lsof -ti:5001 | xargs kill -9
```

### Issue 2: Cannot Connect to Redis
**Error**: "No connection is available"

**Solution**:
```bash
# Check if Redis is running
docker ps | grep redis

# Restart Redis
docker-compose restart redis

# Check logs
docker-compose logs redis
```

### Issue 3: Build Errors
**Error**: "The type or namespace name could not be found"

**Solution**:
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Issue 4: Swagger Not Loading
**Error**: "Cannot GET /swagger"

**Solution**:
- Ensure you're in Development environment
- Check `launchSettings.json` for correct URL
- Verify `app.UseSwagger()` is called in Program.cs

---

## Step 17: Next Steps & Extensions

### Completed ✅
- ✅ Project structure created
- ✅ Core layer with entities
- ✅ Infrastructure with Redis
- ✅ Application with CQRS
- ✅ API with controllers
- ✅ Docker Compose setup
- ✅ Swagger documentation
- ✅ Testing with HTTP requests

### To Complete the Full System

**Repeat similar steps for**:
1. **Flight Service** (MongoDB)
2. **Payment Service** (PostgreSQL)
3. **Notification Service** (In-Memory)

**Each service needs**:
- Core layer (entities + interfaces)
- Infrastructure layer (repository implementation)
- Application layer (commands + queries + handlers)
- API layer (controllers + Program.cs + appsettings.json)

### Advanced Topics to Explore

1. **Add Authentication**
   - JWT tokens
   - Identity Server

2. **Add API Gateway**
   - Ocelot
   - YARP

3. **Add Logging**
   - Serilog
   - Application Insights

4. **Add Monitoring**
   - Prometheus
   - Grafana

5. **Deploy to Cloud**
   - Azure Kubernetes Service
   - AWS ECS
   - Google Kubernetes Engine

---

## Congratulations! 🎉

You've successfully:
- ✅ Created a microservices project structure
- ✅ Implemented clean architecture
- ✅ Used CQRS pattern with MediatR
- ✅ Integrated Redis database
- ✅ Set up event-driven communication
- ✅ Created REST API with Swagger
- ✅ Tested your API

### What You've Learned

1. **Microservices Architecture**: Independent, scalable services
2. **Clean Architecture**: Separation of concerns in layers
3. **CQRS Pattern**: Separate read and write operations
4. **Repository Pattern**: Abstract data access
5. **Dependency Injection**: Loose coupling
6. **Event-Driven**: Asynchronous communication
7. **Docker**: Containerization
8. **REST API**: HTTP-based services

### Resources for Further Learning

- **Official Docs**: https://docs.microsoft.com/aspnet/core
- **MediatR**: https://github.com/jbogard/MediatR
- **MassTransit**: https://masstransit-project.com
- **Clean Architecture**: https://blog.cleancoder.com

### Get the Complete Code

All files are available in the FlightBookingSystem directory. Review:
- **README.md** - Complete documentation
- **ARCHITECTURE.md** - Design details
- **QUICKSTART.md** - Quick reference
- **Actual source files** - Complete implementations

---

## Final Checklist

- [ ] All projects build successfully
- [ ] Docker services are running
- [ ] Booking API is accessible
- [ ] Swagger UI loads
- [ ] Can create a booking
- [ ] Can retrieve a booking
- [ ] Data persists in Redis
- [ ] Understand the complete flow

**If all checked, you're ready to build the other services!** 🚀

---

**Next Tutorial**: [Part 4: AI Service with Gemma Integration](./TUTORIAL_PART4_AI_SERVICE.md) - Learn how to add AI-powered features to your flight booking system!

---

**Happy Coding!** 💻
