# 🎓 Beginner's Tutorial Part 1 - Project Setup & Core Layer

## Overview
This is Part 1 of a complete beginner's tutorial for building a microservices Flight Booking System. This part covers:
- Project structure creation
- Understanding clean architecture
- Creating the Core layer (entities and interfaces)
- Setting up the solution

**Estimated Time**: 1-2 hours  
**Difficulty**: Beginner-Intermediate

---

## Prerequisites

### Required Software
1. **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
3. **IDE**: Visual Studio 2022, VS Code, or JetBrains Rider
4. **Terminal/Command Prompt**

### Verify Installation
```bash
dotnet --version  # Should show 9.0.x
docker --version  # Should show Docker version
```

---

## Step 1: Understanding the Architecture (10 minutes)

### What Are We Building?
A Flight Booking System with 4 microservices:
- **Booking Service**: Manages bookings (uses Redis)
- **Flight Service**: Manages flights (uses MongoDB)
- **Payment Service**: Processes payments (uses PostgreSQL)
- **Notification Service**: Sends notifications (uses In-Memory)

### Clean Architecture Layers
Each service has 4 layers:
```
API Layer         → Controllers, HTTP endpoints
Application Layer → Business logic (CQRS)
Infrastructure    → Database access
Core Layer        → Domain models, interfaces
```

---

## Step 2: Create Project Structure (15 minutes)

### 2.1 Create Root Directory
```bash
mkdir FlightBookingSystem
cd FlightBookingSystem
```

### 2.2 Create Solution File
```bash
dotnet new sln -n FlightBookingSystem
```
**What this does**: Creates a `.sln` file that groups all projects together.

### 2.3 Create Folder Structure
```bash
# BuildingBlocks (shared code)
mkdir -p src/BuildingBlocks/MassTransit
mkdir -p src/BuildingBlocks/RabbitMQ

# Booking Service
mkdir -p src/Services/Booking/Booking.Core
mkdir -p src/Services/Booking/Booking.Infrastructure
mkdir -p src/Services/Booking/Booking.Application
mkdir -p src/Services/Booking/Booking.API

# Flight Service
mkdir -p src/Services/Flight/Flight.Core
mkdir -p src/Services/Flight/Flight.Infrastructure
mkdir -p src/Services/Flight/Flight.Application
mkdir -p src/Services/Flight/Flight.API

# Payment Service
mkdir -p src/Services/Payment/Payment.Core
mkdir -p src/Services/Payment/Payment.Infrastructure
mkdir -p src/Services/Payment/Payment.Application
mkdir -p src/Services/Payment/Payment.API

# Notification Service
mkdir -p src/Services/Notification/Notification.Core
mkdir -p src/Services/Notification/Notification.Infrastructure
mkdir -p src/Services/Notification/Notification.Application
mkdir -p src/Services/Notification/Notification.API

# Tests
mkdir -p tests/Booking.API.Tests
mkdir -p tests/Flight.API.Tests
mkdir -p tests/Payment.API.Tests
mkdir -p tests/Notification.API.Tests
mkdir -p tests/MassTransit.Tests
```

---

## Step 3: Create All Projects (20 minutes)

### 3.1 Create BuildingBlocks Projects
```bash
cd src/BuildingBlocks/MassTransit
dotnet new classlib -n BuildingBlocks.MassTransit -f net9.0

cd ../RabbitMQ
dotnet new classlib -n BuildingBlocks.RabbitMQ -f net9.0
```

### 3.2 Create Booking Service Projects
```bash
cd ../../Services/Booking/Booking.Core
dotnet new classlib -n Booking.Core -f net9.0

cd ../Booking.Infrastructure
dotnet new classlib -n Booking.Infrastructure -f net9.0

cd ../Booking.Application
dotnet new classlib -n Booking.Application -f net9.0

cd ../Booking.API
dotnet new webapi -n Booking.API -f net9.0
```

### 3.3 Repeat for Other Services
```bash
# Flight Service
cd ../../Flight/Flight.Core
dotnet new classlib -n Flight.Core -f net9.0
cd ../Flight.Infrastructure
dotnet new classlib -n Flight.Infrastructure -f net9.0
cd ../Flight.Application
dotnet new classlib -n Flight.Application -f net9.0
cd ../Flight.API
dotnet new webapi -n Flight.API -f net9.0

# Payment Service
cd ../../Payment/Payment.Core
dotnet new classlib -n Payment.Core -f net9.0
cd ../Payment.Infrastructure
dotnet new classlib -n Payment.Infrastructure -f net9.0
cd ../Payment.Application
dotnet new classlib -n Payment.Application -f net9.0
cd ../Payment.API
dotnet new webapi -n Payment.API -f net9.0

# Notification Service
cd ../../Notification/Notification.Core
dotnet new classlib -n Notification.Core -f net9.0
cd ../Notification.Infrastructure
dotnet new classlib -n Notification.Infrastructure -f net9.0
cd ../Notification.Application
dotnet new classlib -n Notification.Application -f net9.0
cd ../Notification.API
dotnet new webapi -n Notification.API -f net9.0
```

### 3.4 Create Test Projects
```bash
cd ../../../../tests/Booking.API.Tests
dotnet new xunit -n Booking.API.Tests -f net9.0

cd ../Flight.API.Tests
dotnet new xunit -n Flight.API.Tests -f net9.0

cd ../Payment.API.Tests
dotnet new xunit -n Payment.API.Tests -f net9.0

cd ../Notification.API.Tests
dotnet new xunit -n Notification.API.Tests -f net9.0

cd ../MassTransit.Tests
dotnet new xunit -n MassTransit.Tests -f net9.0

cd ../../
```

### 3.5 Add All Projects to Solution
```bash
# Add BuildingBlocks
dotnet sln add src/BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj
dotnet sln add src/BuildingBlocks/RabbitMQ/BuildingBlocks.RabbitMQ/BuildingBlocks.RabbitMQ.csproj

# Add Booking Service
dotnet sln add src/Services/Booking/Booking.Core/Booking.Core/Booking.Core.csproj
dotnet sln add src/Services/Booking/Booking.Infrastructure/Booking.Infrastructure/Booking.Infrastructure.csproj
dotnet sln add src/Services/Booking/Booking.Application/Booking.Application/Booking.Application.csproj
dotnet sln add src/Services/Booking/Booking.API/Booking.API/Booking.API.csproj

# Add Flight Service
dotnet sln add src/Services/Flight/Flight.Core/Flight.Core/Flight.Core.csproj
dotnet sln add src/Services/Flight/Flight.Infrastructure/Flight.Infrastructure/Flight.Infrastructure.csproj
dotnet sln add src/Services/Flight/Flight.Application/Flight.Application/Flight.Application.csproj
dotnet sln add src/Services/Flight/Flight.API/Flight.API/Flight.API.csproj

# Add Payment Service
dotnet sln add src/Services/Payment/Payment.Core/Payment.Core/Payment.Core.csproj
dotnet sln add src/Services/Payment/Payment.Infrastructure/Payment.Infrastructure/Payment.Infrastructure.csproj
dotnet sln add src/Services/Payment/Payment.Application/Payment.Application/Payment.Application.csproj
dotnet sln add src/Services/Payment/Payment.API/Payment.API/Payment.API.csproj

# Add Notification Service
dotnet sln add src/Services/Notification/Notification.Core/Notification.Core/Notification.Core.csproj
dotnet sln add src/Services/Notification/Notification.Infrastructure/Notification.Infrastructure/Notification.Infrastructure.csproj
dotnet sln add src/Services/Notification/Notification.Application/Notification.Application/Notification.Application.csproj
dotnet sln add src/Services/Notification/Notification.API/Notification.API/Notification.API.csproj

# Add Tests
dotnet sln add tests/Booking.API.Tests/Booking.API.Tests/Booking.API.Tests.csproj
dotnet sln add tests/Flight.API.Tests/Flight.API.Tests/Flight.API.Tests.csproj
dotnet sln add tests/Payment.API.Tests/Payment.API.Tests/Payment.API.Tests.csproj
dotnet sln add tests/Notification.API.Tests/Notification.API.Tests/Notification.API.Tests.csproj
dotnet sln add tests/MassTransit.Tests/MassTransit.Tests/MassTransit.Tests.csproj
```

### 3.6 Verify Solution
```bash
dotnet build
```
Should build successfully (even though projects are empty).

---

## Step 4: Create Core Layer - Booking Service (30 minutes)

The Core layer contains domain entities and repository interfaces. It has NO dependencies.

### 4.1 Create Booking Entity

**File**: `src/Services/Booking/Booking.Core/Booking.Core/Entities/BookingEntity.cs`

Delete the default `Class1.cs` file and create:

```csharp
namespace Booking.Core.Entities;

public class BookingEntity
{
    public Guid Id { get; set; }
    public Guid FlightId { get; set; }
    public Guid UserId { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerEmail { get; set; } = string.Empty;
    public string PassengerPhone { get; set; } = string.Empty;
    public int NumberOfSeats { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? ConfirmationDate { get; set; }
    public string? BookingReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum BookingStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    PaymentPending = 3,
    PaymentCompleted = 4,
    PaymentFailed = 5
}
```

**Understanding the code**:
- `Guid Id`: Unique identifier (better than int for distributed systems)
- `BookingStatus`: Enum defines valid states
- `DateTime?`: Question mark means nullable (can be null)
- `string.Empty`: Better than null for strings

### 4.2 Create Repository Interface

**File**: `src/Services/Booking/Booking.Core/Booking.Core/Repositories/IBookingRepository.cs`

```csharp
using Booking.Core.Entities;

namespace Booking.Core.Repositories;

public interface IBookingRepository
{
    Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingEntity>> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default);
    Task<BookingEntity?> GetByBookingReferenceAsync(string bookingReference, CancellationToken cancellationToken = default);
    Task<BookingEntity> CreateAsync(BookingEntity booking, CancellationToken cancellationToken = default);
    Task<BookingEntity> UpdateAsync(BookingEntity booking, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

**Understanding the code**:
- `interface`: Contract defining what methods must exist
- `Task<T>`: Async method returning type T
- `CancellationToken`: Allows cancelling long operations
- `default`: Optional parameter with default value

---

## Step 5: Create Core Layer - Flight Service (15 minutes)

### 5.1 Create Flight Entity

**File**: `src/Services/Flight/Flight.Core/Flight.Core/Entities/FlightEntity.cs`

```csharp
namespace Flight.Core.Entities;

public class FlightEntity
{
    public Guid Id { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string Airline { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal PricePerSeat { get; set; }
    public FlightStatus Status { get; set; }
    public string? AircraftType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum FlightStatus
{
    Scheduled = 0,
    Boarding = 1,
    Departed = 2,
    InFlight = 3,
    Landed = 4,
    Cancelled = 5,
    Delayed = 6
}
```

### 5.2 Create Flight Repository Interface

**File**: `src/Services/Flight/Flight.Core/Flight.Core/Repositories/IFlightRepository.cs`

```csharp
using Flight.Core.Entities;

namespace Flight.Core.Repositories;

public interface IFlightRepository
{
    Task<FlightEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> GetByFlightNumberAsync(string flightNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<FlightEntity>> SearchFlightsAsync(string departureAirport, string arrivalAirport, DateTime departureDate, CancellationToken cancellationToken = default);
    Task<FlightEntity> CreateAsync(FlightEntity flight, CancellationToken cancellationToken = default);
    Task<FlightEntity> UpdateAsync(FlightEntity flight, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAvailableSeatsAsync(Guid id, int seats, CancellationToken cancellationToken = default);
}
```

---

## Checkpoint: What We've Accomplished

✅ Created solution structure  
✅ Created 23 projects  
✅ Added projects to solution  
✅ Created Core layer for Booking service  
✅ Created Core layer for Flight service  

**Next**: Continue with TUTORIAL_PART2.md for Infrastructure and Application layers.

---

## Common Issues & Solutions

**Issue**: "dotnet command not found"  
**Solution**: Install .NET 9.0 SDK and restart terminal

**Issue**: "Project already exists"  
**Solution**: Delete the folder and try again

**Issue**: "Build failed"  
**Solution**: Make sure all namespaces match folder structure

---

## Next Steps

Continue to **TUTORIAL_PART2.md** to learn:
- Creating Infrastructure layer with databases
- Implementing CQRS with MediatR
- Setting up message brokers
- Creating API controllers
