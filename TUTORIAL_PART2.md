# 🎓 Beginner's Tutorial Part 2 - Infrastructure & Application Layers

## Overview
This is Part 2 of the complete beginner's tutorial. This part covers:
- Creating Infrastructure layer with database implementations
- Understanding different databases (Redis, MongoDB, PostgreSQL)
- Creating Application layer with CQRS pattern
- Setting up MassTransit for messaging

**Estimated Time**: 2-3 hours  
**Difficulty**: Intermediate

---

## Step 6: Create Infrastructure Layer - Booking Service (Redis) (30 minutes)

### 6.1 Add NuGet Packages

```bash
cd src/Services/Booking/Booking.Infrastructure/Booking.Infrastructure
dotnet add package StackExchange.Redis --version 2.7.33
dotnet add reference ../../Booking.Core/Booking.Core/Booking.Core.csproj
```

**What StackExchange.Redis does**: Client library to connect to Redis database.

### 6.2 Create Booking Repository Implementation

**File**: `src/Services/Booking/Booking.Infrastructure/Booking.Infrastructure/Repositories/BookingRepository.cs`

Delete `Class1.cs` and create:

```csharp
using Booking.Core.Entities;
using Booking.Core.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace Booking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly IDatabase _database;
    private const string KeyPrefix = "booking:";

    public BookingRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync($"{KeyPrefix}{id}");
        return value.HasValue ? JsonSerializer.Deserialize<BookingEntity>(value!) : null;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{KeyPrefix}*").ToList();
        
        var bookings = new List<BookingEntity>();
        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
            {
                var booking = JsonSerializer.Deserialize<BookingEntity>(value!);
                if (booking != null) bookings.Add(booking);
            }
        }
        return bookings;
    }

    public async Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.Where(b => b.UserId == userId);
    }

    public async Task<IEnumerable<BookingEntity>> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.Where(b => b.FlightId == flightId);
    }

    public async Task<BookingEntity?> GetByBookingReferenceAsync(string bookingReference, CancellationToken cancellationToken = default)
    {
        var allBookings = await GetAllAsync(cancellationToken);
        return allBookings.FirstOrDefault(b => b.BookingReference == bookingReference);
    }

    public async Task<BookingEntity> CreateAsync(BookingEntity booking, CancellationToken cancellationToken = default)
    {
        booking.Id = Guid.NewGuid();
        booking.CreatedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(booking);
        await _database.StringSetAsync($"{KeyPrefix}{booking.Id}", json);
        return booking;
    }

    public async Task<BookingEntity> UpdateAsync(BookingEntity booking, CancellationToken cancellationToken = default)
    {
        booking.UpdatedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(booking);
        await _database.StringSetAsync($"{KeyPrefix}{booking.Id}", json);
        return booking;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _database.KeyDeleteAsync($"{KeyPrefix}{id}");
    }
}
```

**Understanding the code**:
- **Redis stores key-value pairs**: We use `booking:{id}` as key
- **JSON serialization**: Convert objects to JSON strings for storage
- **StringGetAsync/StringSetAsync**: Redis commands to get/set values
- **KeyPrefix**: Helps organize data in Redis

### 6.3 Create Dependency Injection Configuration

**File**: `src/Services/Booking/Booking.Infrastructure/Booking.Infrastructure/DependencyInjection.cs`

```csharp
using Booking.Core.Repositories;
using Booking.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}
```

**Understanding the code**:
- **Extension method**: Adds `AddBookingInfrastructure()` to IServiceCollection
- **Singleton**: One Redis connection for entire application
- **Scoped**: One repository instance per HTTP request
- **Configuration**: Reads connection string from appsettings.json

---

## Step 7: Create Infrastructure Layer - Flight Service (MongoDB) (30 minutes)

### 7.1 Add NuGet Packages

```bash
cd ../../../Flight/Flight.Infrastructure/Flight.Infrastructure
dotnet add package MongoDB.Driver --version 2.23.1
dotnet add reference ../../Flight.Core/Flight.Core/Flight.Core.csproj
```

### 7.2 Create Flight Repository Implementation

**File**: `src/Services/Flight/Flight.Infrastructure/Flight.Infrastructure/Repositories/FlightRepository.cs`

```csharp
using Flight.Core.Entities;
using Flight.Core.Repositories;
using MongoDB.Driver;

namespace Flight.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly IMongoCollection<FlightEntity> _flights;

    public FlightRepository(IMongoDatabase database)
    {
        _flights = database.GetCollection<FlightEntity>("flights");
    }

    public async Task<FlightEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(f => f.Id == id, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(_ => true, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default)
    {
        var cursor = await _flights.FindAsync(f => f.FlightNumber == flightNumber, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlightEntity>> SearchFlightsAsync(string departureAirport, string arrivalAirport, DateTime departureDate, CancellationToken cancellationToken = default)
    {
        var startDate = departureDate.Date;
        var endDate = startDate.AddDays(1);

        var filter = Builders<FlightEntity>.Filter.And(
            Builders<FlightEntity>.Filter.Eq(f => f.DepartureAirport, departureAirport),
            Builders<FlightEntity>.Filter.Eq(f => f.ArrivalAirport, arrivalAirport),
            Builders<FlightEntity>.Filter.Gte(f => f.DepartureTime, startDate),
            Builders<FlightEntity>.Filter.Lt(f => f.DepartureTime, endDate)
        );

        var cursor = await _flights.FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<FlightEntity> CreateAsync(FlightEntity flight, CancellationToken cancellationToken = default)
    {
        flight.Id = Guid.NewGuid();
        flight.CreatedAt = DateTime.UtcNow;
        await _flights.InsertOneAsync(flight, cancellationToken: cancellationToken);
        return flight;
    }

    public async Task<FlightEntity> UpdateAsync(FlightEntity flight, CancellationToken cancellationToken = default)
    {
        flight.UpdatedAt = DateTime.UtcNow;
        await _flights.ReplaceOneAsync(f => f.Id == flight.Id, flight, cancellationToken: cancellationToken);
        return flight;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _flights.DeleteOneAsync(f => f.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> UpdateAvailableSeatsAsync(Guid id, int seats, CancellationToken cancellationToken = default)
    {
        var update = Builders<FlightEntity>.Update
            .Set(f => f.AvailableSeats, seats)
            .Set(f => f.UpdatedAt, DateTime.UtcNow);

        var result = await _flights.UpdateOneAsync(f => f.Id == id, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}
```

**Understanding MongoDB**:
- **Document database**: Stores data as JSON-like documents
- **Collection**: Like a table in SQL (we use "flights" collection)
- **Filter builders**: MongoDB's way to build queries
- **FindAsync**: Query documents
- **InsertOneAsync**: Add new document

### 7.3 Create Dependency Injection Configuration

**File**: `src/Services/Flight/Flight.Infrastructure/Flight.Infrastructure/DependencyInjection.cs`

```csharp
using Flight.Core.Repositories;
using Flight.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Flight.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFlightInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "FlightDB";

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        services.AddScoped<IFlightRepository, FlightRepository>();

        return services;
    }
}
```

---

## Step 8: Create Application Layer - Booking Service (45 minutes)

### 8.1 Add NuGet Packages

```bash
cd ../../../Booking/Booking.Application/Booking.Application
dotnet add package MediatR --version 12.2.0
dotnet add package MassTransit --version 8.2.0
dotnet add reference ../../Booking.Core/Booking.Core/Booking.Core.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj
```

**What these packages do**:
- **MediatR**: Implements CQRS pattern (Commands and Queries)
- **MassTransit**: Message broker for event-driven communication

### 8.2 Create Command (Write Operation)

**File**: `src/Services/Booking/Booking.Application/Booking.Application/Commands/CreateBookingCommand.cs`

```csharp
using MediatR;

namespace Booking.Application.Commands;

public record CreateBookingCommand : IRequest<CreateBookingCommandResponse>
{
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public string PassengerPhone { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
}

public record CreateBookingCommandResponse
{
    public Guid BookingId { get; init; }
    public string BookingReference { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
```

**Understanding CQRS**:
- **Command**: Represents an action that changes data
- **IRequest<T>**: MediatR interface for requests
- **record**: Immutable data structure (C# 9+)

### 8.3 Create Command Handler

**File**: `src/Services/Booking/Booking.Application/Booking.Application/Commands/CreateBookingCommandHandler.cs`

```csharp
using Booking.Core.Entities;
using Booking.Core.Repositories;
using BuildingBlocks.MassTransit.Messages;
using MassTransit;
using MediatR;

namespace Booking.Application.Commands;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingCommandResponse>
{
    private readonly IBookingRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateBookingCommandHandler(IBookingRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateBookingCommandResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Create booking entity
        var booking = new BookingEntity
        {
            FlightId = request.FlightId,
            UserId = request.UserId,
            PassengerName = request.PassengerName,
            PassengerEmail = request.PassengerEmail,
            PassengerPhone = request.PassengerPhone,
            NumberOfSeats = request.NumberOfSeats,
            TotalAmount = request.TotalAmount,
            Status = BookingStatus.Pending,
            BookingDate = DateTime.UtcNow,
            BookingReference = GenerateBookingReference()
        };

        // Step 2: Save to database
        var createdBooking = await _repository.CreateAsync(booking, cancellationToken);

        // Step 3: Publish event to RabbitMQ
        await _publishEndpoint.Publish(new BookingCreatedEvent
        {
            BookingId = createdBooking.Id,
            FlightId = createdBooking.FlightId,
            UserId = createdBooking.UserId,
            PassengerName = createdBooking.PassengerName,
            PassengerEmail = createdBooking.PassengerEmail,
            NumberOfSeats = createdBooking.NumberOfSeats,
            TotalAmount = createdBooking.TotalAmount,
            BookingDate = createdBooking.BookingDate
        }, cancellationToken);

        // Step 4: Return response
        return new CreateBookingCommandResponse
        {
            BookingId = createdBooking.Id,
            BookingReference = createdBooking.BookingReference!,
            Status = createdBooking.Status.ToString()
        };
    }

    private static string GenerateBookingReference()
    {
        return $"BK{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }
}
```

**Understanding the flow**:
1. **Receive command**: Handler gets CreateBookingCommand
2. **Create entity**: Convert command to BookingEntity
3. **Save to database**: Use repository
4. **Publish event**: Notify other services via RabbitMQ
5. **Return response**: Send result back to caller

### 8.4 Create Query (Read Operation)

**File**: `src/Services/Booking/Booking.Application/Booking.Application/Queries/GetBookingByIdQuery.cs`

```csharp
using MediatR;

namespace Booking.Application.Queries;

public record GetBookingByIdQuery(Guid BookingId) : IRequest<GetBookingByIdQueryResponse?>;

public record GetBookingByIdQueryResponse
{
    public Guid Id { get; init; }
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public string PassengerPhone { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime BookingDate { get; init; }
    public string? BookingReference { get; init; }
}
```

### 8.5 Create Query Handler

**File**: `src/Services/Booking/Booking.Application/Booking.Application/Queries/GetBookingByIdQueryHandler.cs`

```csharp
using Booking.Core.Repositories;
using MediatR;

namespace Booking.Application.Queries;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, GetBookingByIdQueryResponse?>
{
    private readonly IBookingRepository _repository;

    public GetBookingByIdQueryHandler(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetBookingByIdQueryResponse?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(request.BookingId, cancellationToken);
        
        if (booking == null)
            return null;

        return new GetBookingByIdQueryResponse
        {
            Id = booking.Id,
            FlightId = booking.FlightId,
            UserId = booking.UserId,
            PassengerName = booking.PassengerName,
            PassengerEmail = booking.PassengerEmail,
            PassengerPhone = booking.PassengerPhone,
            NumberOfSeats = booking.NumberOfSeats,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status.ToString(),
            BookingDate = booking.BookingDate,
            BookingReference = booking.BookingReference
        };
    }
}
```

**Understanding Queries**:
- **Read-only**: Queries don't modify data
- **No side effects**: Safe to call multiple times
- **DTO mapping**: Convert entity to response DTO

### 8.6 Create Dependency Injection Configuration

**File**: `src/Services/Booking/Booking.Application/Booking.Application/DependencyInjection.cs`

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBookingApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        return services;
    }
}
```

---

## Step 9: Create BuildingBlocks - MassTransit (30 minutes)

### 9.1 Add NuGet Packages

```bash
cd ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit
dotnet add package MassTransit --version 8.2.0
dotnet add package MassTransit.RabbitMQ --version 8.2.0
```

### 9.2 Create Message Contracts

**File**: `src/BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/Messages/BookingCreatedEvent.cs`

```csharp
namespace BuildingBlocks.MassTransit.Messages;

public record BookingCreatedEvent
{
    public Guid BookingId { get; init; }
    public Guid FlightId { get; init; }
    public Guid UserId { get; init; }
    public string PassengerName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
    public int NumberOfSeats { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime BookingDate { get; init; }
}
```

**Why events**:
- **Decoupling**: Services don't directly call each other
- **Asynchronous**: Non-blocking communication
- **Scalability**: Easy to add new subscribers

### 9.3 Create MassTransit Configuration

**File**: `src/BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/Configuration/MassTransitConfiguration.cs`

```csharp
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.MassTransit.Configuration;

public static class MassTransitConfiguration
{
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                var rabbitMqPort = configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
                var rabbitMqUsername = configuration["RabbitMQ:Username"] ?? "guest";
                var rabbitMqPassword = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(rabbitMqHost, rabbitMqPort, "/", h =>
                {
                    h.Username(rabbitMqUsername);
                    h.Password(rabbitMqPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
```

---

## Checkpoint: What We've Accomplished

✅ Created Infrastructure layer for Booking (Redis)  
✅ Created Infrastructure layer for Flight (MongoDB)  
✅ Created Application layer with CQRS  
✅ Set up MassTransit for messaging  
✅ Created message contracts  

**Next**: Continue with TUTORIAL_PART3.md for API layer and testing.

---

## Key Concepts Learned

1. **Repository Pattern**: Abstracts data access
2. **Dependency Injection**: Loose coupling between layers
3. **CQRS**: Separate read and write operations
4. **Event-Driven**: Services communicate via events
5. **Multiple Databases**: Each service uses appropriate database

---

## Next Steps

Continue to **TUTORIAL_PART3.md** to learn:
- Creating API controllers
- Setting up Swagger
- Configuring appsettings.json
- Running and testing the system
