# Flight Booking System - Architecture Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [Microservices Architecture](#microservices-architecture)
3. [Clean Architecture Layers](#clean-architecture-layers)
4. [Database Strategy](#database-strategy)
5. [Message Broker Pattern](#message-broker-pattern)
6. [CQRS Pattern](#cqrs-pattern)
7. [Event-Driven Communication](#event-driven-communication)
8. [Scalability Considerations](#scalability-considerations)

## System Overview

The Flight Booking System is a distributed microservices application designed to handle flight bookings, payments, and notifications. The system follows Domain-Driven Design (DDD) principles and implements clean architecture across all services.

### Key Design Principles
- **Separation of Concerns**: Each microservice handles a specific business domain
- **Independence**: Services can be deployed, scaled, and updated independently
- **Resilience**: Failure in one service doesn't cascade to others
- **Technology Diversity**: Each service uses the most appropriate database technology

## Microservices Architecture

### 1. Booking Service
**Responsibility**: Manages flight booking lifecycle

**Technology Stack**:
- Database: Redis (In-memory key-value store)
- Messaging: MassTransit + RabbitMQ

**Why Redis?**
- Fast read/write operations for booking data
- Built-in expiration for temporary bookings
- Atomic operations for seat reservations
- Easy horizontal scaling

**Key Features**:
- Create bookings
- Retrieve booking details
- Update booking status
- Generate booking references

### 2. Flight Service
**Responsibility**: Manages flight information and availability

**Technology Stack**:
- Database: MongoDB (Document database)
- Messaging: MassTransit + RabbitMQ

**Why MongoDB?**
- Flexible schema for flight data
- Excellent query performance for search operations
- Horizontal scalability through sharding
- Rich query language for complex searches

**Key Features**:
- Create and manage flights
- Search flights by route and date
- Update seat availability
- Track flight status

### 3. Payment Service
**Responsibility**: Processes payment transactions

**Technology Stack**:
- Database: PostgreSQL (Relational database)
- Messaging: MassTransit + RabbitMQ

**Why PostgreSQL?**
- ACID compliance for financial transactions
- Strong consistency guarantees
- Complex transaction support
- Mature ecosystem

**Key Features**:
- Process payments
- Track payment status
- Generate transaction IDs
- Payment history retrieval

### 4. Notification Service
**Responsibility**: Sends notifications to users

**Technology Stack**:
- Database: In-Memory (Development), can be replaced with any persistent store
- Messaging: MassTransit + RabbitMQ (Consumer)

**Key Features**:
- Email notifications
- SMS notifications (extensible)
- Push notifications (extensible)
- Notification history

## Clean Architecture Layers

Each microservice follows a 4-layer clean architecture:

### 1. Core Layer (Domain)
**Purpose**: Contains business entities and repository interfaces

**Characteristics**:
- No external dependencies
- Pure domain logic
- Entity definitions
- Repository interfaces

**Example**:
```
Booking.Core/
├── Entities/
│   └── BookingEntity.cs
└── Repositories/
    └── IBookingRepository.cs
```

### 2. Infrastructure Layer
**Purpose**: Implements data access and external service integrations

**Characteristics**:
- Database implementations
- External API clients
- File system access
- Depends on Core layer

**Example**:
```
Booking.Infrastructure/
├── Repositories/
│   └── BookingRepository.cs
└── DependencyInjection.cs
```

### 3. Application Layer
**Purpose**: Contains business logic and use cases

**Characteristics**:
- CQRS handlers
- Business rules
- Validation logic
- Depends on Core layer

**Example**:
```
Booking.Application/
├── Commands/
│   ├── CreateBookingCommand.cs
│   └── CreateBookingCommandHandler.cs
├── Queries/
│   ├── GetBookingByIdQuery.cs
│   └── GetBookingByIdQueryHandler.cs
└── DependencyInjection.cs
```

### 4. API Layer (Presentation)
**Purpose**: Exposes HTTP endpoints

**Characteristics**:
- REST controllers
- Request/response models
- API documentation
- Depends on Application layer

**Example**:
```
Booking.API/
├── Controllers/
│   └── BookingsController.cs
├── Program.cs
└── appsettings.json
```

## Database Strategy

### Polyglot Persistence
The system uses different databases optimized for each service's needs:

| Service | Database | Reason |
|---------|----------|--------|
| Booking | Redis | Fast, in-memory, TTL support |
| Flight | MongoDB | Flexible schema, search performance |
| Payment | PostgreSQL | ACID, consistency, transactions |
| Notification | In-Memory | Temporary storage, can be replaced |

### Data Consistency
- **Eventual Consistency**: Services communicate via events
- **Saga Pattern**: Can be implemented for distributed transactions
- **Idempotency**: All operations are idempotent

## Message Broker Pattern

### RabbitMQ + MassTransit
**Why MassTransit?**
- Abstraction over message brokers
- Built-in retry and error handling
- Message routing and correlation
- Testing support

### Message Types

#### 1. Events (Publish/Subscribe)
- `BookingCreatedEvent`: Published when booking is created
- `PaymentProcessedEvent`: Published when payment completes
- `FlightSeatsUpdatedEvent`: Published when seats are updated

#### 2. Commands (Send/Receive)
- `SendNotificationCommand`: Direct command to notification service

### Message Flow Example
```
User → Booking API → BookingCreatedEvent → RabbitMQ → Notification Service
                                         → Payment Service
```

## CQRS Pattern

### Command Query Responsibility Segregation

**Commands** (Write Operations):
- Modify state
- Return simple acknowledgment
- Can trigger events

**Queries** (Read Operations):
- Read-only operations
- Return data
- No side effects

### Benefits
- Optimized read and write models
- Scalability (scale reads and writes independently)
- Clear separation of concerns
- Easier testing

### Implementation with MediatR
```csharp
// Command
public record CreateBookingCommand : IRequest<CreateBookingCommandResponse>
{
    public Guid FlightId { get; init; }
    // ... other properties
}

// Query
public record GetBookingByIdQuery(Guid BookingId) 
    : IRequest<GetBookingByIdQueryResponse?>;
```

## Event-Driven Communication

### Asynchronous Messaging
Services communicate asynchronously through events:

1. **Booking Created Flow**:
   ```
   Booking Service → BookingCreatedEvent → Notification Service
                                        → Flight Service (update seats)
   ```

2. **Payment Processed Flow**:
   ```
   Payment Service → PaymentProcessedEvent → Notification Service
                                           → Booking Service (update status)
   ```

### Benefits
- Loose coupling between services
- Better fault tolerance
- Improved scalability
- Easier to add new services

## Scalability Considerations

### Horizontal Scaling
- Each microservice can be scaled independently
- Stateless design allows multiple instances
- Load balancing at API Gateway level

### Database Scaling
- **Redis**: Cluster mode for Booking service
- **MongoDB**: Sharding for Flight service
- **PostgreSQL**: Read replicas for Payment service

### Message Broker Scaling
- RabbitMQ clustering
- Queue partitioning
- Consumer groups

### Caching Strategy
- Redis for distributed caching
- In-memory caching for frequently accessed data
- Cache invalidation via events

### Performance Optimization
- Connection pooling
- Async/await throughout
- Bulk operations where applicable
- Indexed database queries

## Security Considerations

### Future Enhancements
- JWT authentication
- API Gateway (Ocelot/YARP)
- Rate limiting
- HTTPS enforcement
- Secret management (Azure Key Vault, AWS Secrets Manager)

## Monitoring and Observability

### Recommended Tools
- **Logging**: Serilog with structured logging
- **Tracing**: OpenTelemetry
- **Metrics**: Prometheus + Grafana
- **Health Checks**: ASP.NET Core Health Checks
- **APM**: Application Insights or Elastic APM

## Deployment Strategy

### Container Orchestration
- Docker containers for each service
- Kubernetes for orchestration
- Helm charts for deployment
- CI/CD with GitHub Actions or Azure DevOps

### Environment Configuration
- Development: Local Docker Compose
- Staging: Kubernetes cluster
- Production: Managed Kubernetes (AKS, EKS, GKE)

## Testing Strategy

### Test Pyramid
1. **Unit Tests**: Test individual components
2. **Integration Tests**: Test service interactions
3. **End-to-End Tests**: Test complete workflows
4. **Performance Tests**: Load and stress testing

### Test Coverage
- Controllers: Mocked dependencies
- Handlers: Business logic validation
- Repositories: Database operations
- Message Consumers: Event handling

## Conclusion

This architecture provides:
- **Scalability**: Independent service scaling
- **Maintainability**: Clear separation of concerns
- **Reliability**: Fault isolation and resilience
- **Flexibility**: Easy to add new features and services
- **Performance**: Optimized database choices per service

The system is designed to grow with business needs while maintaining code quality and system reliability.
