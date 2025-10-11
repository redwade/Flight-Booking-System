# Flight Booking System - Microservices Architecture

A comprehensive flight booking system built with .NET 9.0 microservices architecture, featuring clean architecture principles, CQRS pattern, event-driven communication, and multiple database technologies.

## 🏗️ Architecture Overview

### Microservices
- **Booking Service**: Manages flight bookings (Redis)
- **Flight Service**: Handles flight information and availability (MongoDB)
- **Payment Service**: Processes payments (PostgreSQL)
- **Notification Service**: Sends notifications to users (In-Memory)

### Architecture Layers
Each microservice follows clean architecture with:
- **Core Layer**: Domain entities and repository interfaces
- **Infrastructure Layer**: Database implementations and external services
- **Application Layer**: CQRS handlers (Commands/Queries)
- **API Layer**: REST API controllers

### Building Blocks
- **MassTransit**: Message broker abstraction
- **RabbitMQ**: Message queue for inter-service communication

## 🛠️ Technology Stack

- **.NET 9.0**: Latest .NET framework
- **MediatR**: CQRS implementation
- **MassTransit**: Distributed application framework
- **RabbitMQ**: Message broker
- **MongoDB**: Document database for Flight service
- **Redis**: In-memory data store for Booking service
- **PostgreSQL**: Relational database for Payment service
- **xUnit**: Testing framework
- **Moq**: Mocking framework
- **Swagger/OpenAPI**: API documentation

## 📁 Project Structure

```
FlightBookingSystem/
├── src/
│   ├── BuildingBlocks/
│   │   ├── MassTransit/           # MassTransit configuration and messages
│   │   └── RabbitMQ/              # RabbitMQ settings
│   └── Services/
│       ├── Booking/
│       │   ├── Booking.Core/      # Entities and interfaces
│       │   ├── Booking.Infrastructure/  # Redis repository
│       │   ├── Booking.Application/     # Commands and queries
│       │   └── Booking.API/       # REST API
│       ├── Flight/
│       │   ├── Flight.Core/
│       │   ├── Flight.Infrastructure/   # MongoDB repository
│       │   ├── Flight.Application/
│       │   └── Flight.API/
│       ├── Payment/
│       │   ├── Payment.Core/
│       │   ├── Payment.Infrastructure/  # PostgreSQL repository
│       │   ├── Payment.Application/
│       │   └── Payment.API/
│       └── Notification/
│           ├── Notification.Core/
│           ├── Notification.Infrastructure/
│           ├── Notification.Application/  # Event consumers
│           └── Notification.API/
├── tests/
│   ├── Booking.API.Tests/
│   ├── Flight.API.Tests/
│   ├── Payment.API.Tests/
│   ├── Notification.API.Tests/
│   └── MassTransit.Tests/
├── docker-compose.yml
└── FlightBookingSystem.sln
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- Visual Studio 2022 / VS Code / Rider

### Running with Docker Compose

1. **Start all services and dependencies**:
```bash
docker-compose up -d
```

This will start:
- RabbitMQ (ports 5672, 15672)
- MongoDB (port 27017)
- Redis (port 6379)
- PostgreSQL (port 5432)
- All microservices APIs

2. **Access the services**:
- Booking API: http://localhost:5001/swagger
- Flight API: http://localhost:5002/swagger
- Payment API: http://localhost:5003/swagger
- Notification API: http://localhost:5004/swagger
- RabbitMQ Management: http://localhost:15672 (guest/guest)

### Running Locally (Development)

1. **Start infrastructure services**:
```bash
docker-compose up -d rabbitmq mongodb redis postgres
```

2. **Run each microservice**:
```bash
# Booking API
cd src/Services/Booking/Booking.API/Booking.API
dotnet run

# Flight API
cd src/Services/Flight/Flight.API/Flight.API
dotnet run

# Payment API
cd src/Services/Payment/Payment.API/Payment.API
dotnet run

# Notification API
cd src/Services/Notification/Notification.API/Notification.API
dotnet run
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Booking.API.Tests/Booking.API.Tests/Booking.API.Tests.csproj
dotnet test tests/Flight.API.Tests/Flight.API.Tests/Flight.API.Tests.csproj
dotnet test tests/Payment.API.Tests/Payment.API.Tests/Payment.API.Tests.csproj
dotnet test tests/MassTransit.Tests/MassTransit.Tests/MassTransit.Tests.csproj
```

## 📡 API Endpoints

### Booking API (Port 5001)
- `POST /api/bookings` - Create a new booking
- `GET /api/bookings/{id}` - Get booking by ID
- `GET /api/bookings/health` - Health check

### Flight API (Port 5002)
- `POST /api/flights` - Create a new flight
- `GET /api/flights/search` - Search flights
- `GET /api/flights/health` - Health check

### Payment API (Port 5003)
- `POST /api/payments` - Process payment
- `GET /api/payments/booking/{bookingId}` - Get payments by booking ID
- `GET /api/payments/health` - Health check

### Notification API (Port 5004)
- `GET /api/notifications/user/{userId}` - Get notifications by user ID
- `GET /api/notifications/booking/{bookingId}` - Get notifications by booking ID
- `GET /api/notifications/pending` - Get pending notifications
- `GET /api/notifications/health` - Health check

## 🔄 Event Flow

1. **Booking Created**:
   - User creates a booking via Booking API
   - `BookingCreatedEvent` is published to RabbitMQ
   - Notification service consumes the event and sends confirmation email

2. **Payment Processed**:
   - User processes payment via Payment API
   - `PaymentProcessedEvent` is published to RabbitMQ
   - Notification service consumes the event and sends payment confirmation
   - Booking service updates booking status

3. **Flight Seats Updated**:
   - When booking is confirmed, Flight service updates available seats
   - `FlightSeatsUpdatedEvent` is published to RabbitMQ

## 🗄️ Database Schemas

### Booking Service (Redis)
- Key-Value store with booking data serialized as JSON
- Key pattern: `booking:{guid}`

### Flight Service (MongoDB)
- Collection: `flights`
- Document structure includes flight details, seats, and pricing

### Payment Service (PostgreSQL)
- Table: `Payments`
- Columns: Id, BookingId, UserId, Amount, Currency, PaymentMethod, Status, TransactionId, etc.

### Notification Service (In-Memory)
- ConcurrentDictionary for development/testing
- Can be replaced with a persistent store in production

## 🧪 Testing Strategy

- **Unit Tests**: Test individual components (controllers, handlers, repositories)
- **Integration Tests**: Test MassTransit message publishing and consumption
- **API Tests**: Test HTTP endpoints with mocked dependencies

## 🔧 Configuration

Each microservice has its own `appsettings.json` with:
- Database connection strings
- RabbitMQ configuration
- Logging settings

Example configuration files are included in each API project.

## 📊 Monitoring and Debugging

### RabbitMQ Management Console
- URL: http://localhost:15672
- Username: guest
- Password: guest
- Monitor queues, exchanges, and message flow

### Health Checks
Each API has a `/health` endpoint for monitoring service status.

## 🚧 Future Enhancements

- [ ] Add API Gateway (Ocelot/YARP)
- [ ] Implement authentication and authorization (JWT)
- [ ] Add distributed tracing (OpenTelemetry)
- [ ] Implement circuit breaker pattern (Polly)
- [ ] Add caching layer
- [ ] Implement saga pattern for distributed transactions
- [ ] Add Kubernetes deployment manifests
- [ ] Implement rate limiting
- [ ] Add comprehensive logging (Serilog)
- [ ] Implement health checks dashboard

## 📝 License

This project is for educational purposes.

## 👥 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📞 Support

For issues and questions, please create an issue in the repository.
