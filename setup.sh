#!/bin/bash

echo "=========================================="
echo "Flight Booking System - Setup Script"
echo "=========================================="
echo ""

# Check if .NET 9.0 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install .NET 9.0 SDK first."
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"
echo ""

# Restore all projects
echo "📦 Restoring NuGet packages..."
dotnet restore FlightBookingSystem.sln

if [ $? -eq 0 ]; then
    echo "✅ NuGet packages restored successfully"
else
    echo "❌ Failed to restore NuGet packages"
    exit 1
fi

echo ""

# Build the solution
echo "🔨 Building solution..."
dotnet build FlightBookingSystem.sln --no-restore

if [ $? -eq 0 ]; then
    echo "✅ Solution built successfully"
else
    echo "❌ Failed to build solution"
    exit 1
fi

echo ""

# Run tests
echo "🧪 Running tests..."
dotnet test FlightBookingSystem.sln --no-build --verbosity normal

if [ $? -eq 0 ]; then
    echo "✅ All tests passed"
else
    echo "⚠️  Some tests failed"
fi

echo ""
echo "=========================================="
echo "Setup Complete!"
echo "=========================================="
echo ""
echo "Next steps:"
echo "1. Start infrastructure: docker-compose up -d rabbitmq mongodb redis postgres"
echo "2. Run microservices individually or use: docker-compose up -d"
echo "3. Access APIs:"
echo "   - Booking API: http://localhost:5001/swagger"
echo "   - Flight API: http://localhost:5002/swagger"
echo "   - Payment API: http://localhost:5003/swagger"
echo "   - Notification API: http://localhost:5004/swagger"
echo "   - RabbitMQ Management: http://localhost:15672 (guest/guest)"
echo ""
