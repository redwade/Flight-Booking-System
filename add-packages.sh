#!/bin/bash

echo "Adding NuGet packages and project references..."

# Flight Service
echo "Configuring Flight Service..."
cd src/Services/Flight/Flight.Infrastructure/Flight.Infrastructure
dotnet add package MongoDB.Driver --version 2.23.1
dotnet add reference ../../Flight.Core/Flight.Core/Flight.Core.csproj

cd ../../../Flight/Flight.Application/Flight.Application
dotnet add package MediatR --version 12.2.0
dotnet add reference ../../Flight.Core/Flight.Core/Flight.Core.csproj

cd ../../../Flight/Flight.API/Flight.API
dotnet add reference ../../Flight.Core/Flight.Core/Flight.Core.csproj
dotnet add reference ../../Flight.Application/Flight.Application/Flight.Application.csproj
dotnet add reference ../../Flight.Infrastructure/Flight.Infrastructure/Flight.Infrastructure.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

# Payment Service
echo "Configuring Payment Service..."
cd ../../../Payment/Payment.Infrastructure/Payment.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add reference ../../Payment.Core/Payment.Core/Payment.Core.csproj

cd ../../../Payment/Payment.Application/Payment.Application
dotnet add package MediatR --version 12.2.0
dotnet add package MassTransit --version 8.2.0
dotnet add reference ../../Payment.Core/Payment.Core/Payment.Core.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

cd ../../../Payment/Payment.API/Payment.API
dotnet add reference ../../Payment.Core/Payment.Core/Payment.Core.csproj
dotnet add reference ../../Payment.Application/Payment.Application/Payment.Application.csproj
dotnet add reference ../../Payment.Infrastructure/Payment.Infrastructure/Payment.Infrastructure.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

# Notification Service
echo "Configuring Notification Service..."
cd ../../../Notification/Notification.Infrastructure/Notification.Infrastructure
dotnet add reference ../../Notification.Core/Notification.Core/Notification.Core.csproj

cd ../../../Notification/Notification.Application/Notification.Application
dotnet add package MediatR --version 12.2.0
dotnet add package MassTransit --version 8.2.0
dotnet add reference ../../Notification.Core/Notification.Core/Notification.Core.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

cd ../../../Notification/Notification.API/Notification.API
dotnet add reference ../../Notification.Core/Notification.Core/Notification.Core.csproj
dotnet add reference ../../Notification.Application/Notification.Application/Notification.Application.csproj
dotnet add reference ../../Notification.Infrastructure/Notification.Infrastructure/Notification.Infrastructure.csproj
dotnet add reference ../../../../BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

# Test Projects
echo "Configuring Test Projects..."
cd ../../../../../../tests/Booking.API.Tests/Booking.API.Tests
dotnet add package Moq --version 4.20.70
dotnet add reference ../../../src/Services/Booking/Booking.API/Booking.API/Booking.API.csproj
dotnet add reference ../../../src/Services/Booking/Booking.Application/Booking.Application/Booking.Application.csproj

cd ../../Flight.API.Tests/Flight.API.Tests
dotnet add package Moq --version 4.20.70
dotnet add reference ../../../src/Services/Flight/Flight.API/Flight.API/Flight.API.csproj
dotnet add reference ../../../src/Services/Flight/Flight.Application/Flight.Application/Flight.Application.csproj

cd ../../Payment.API.Tests/Payment.API.Tests
dotnet add package Moq --version 4.20.70
dotnet add reference ../../../src/Services/Payment/Payment.API/Payment.API/Payment.API.csproj
dotnet add reference ../../../src/Services/Payment/Payment.Application/Payment.Application/Payment.Application.csproj

cd ../../MassTransit.Tests/MassTransit.Tests
dotnet add package MassTransit.TestFramework --version 8.2.0
dotnet add reference ../../../src/BuildingBlocks/MassTransit/BuildingBlocks.MassTransit/BuildingBlocks.MassTransit.csproj

cd ../../../

echo "✅ All packages and references added successfully!"
echo "Run 'dotnet build' to build the solution"
