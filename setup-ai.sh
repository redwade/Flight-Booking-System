#!/bin/bash

echo "=========================================="
echo "Flight Booking System - AI Service Setup"
echo "=========================================="
echo ""

# Check if Ollama is installed
if ! command -v ollama &> /dev/null
then
    echo "❌ Ollama is not installed."
    echo ""
    echo "Please install Ollama from: https://ollama.ai"
    echo ""
    echo "Installation instructions:"
    echo "  macOS/Linux: curl -fsSL https://ollama.ai/install.sh | sh"
    echo "  Windows: Download from https://ollama.ai/download"
    echo ""
    exit 1
fi

echo "✅ Ollama is installed"
echo ""

# Check if Ollama is running
if ! curl -s http://localhost:11434/api/tags > /dev/null 2>&1
then
    echo "⚠️  Ollama is not running. Starting Ollama..."
    echo ""
    echo "Please run in a separate terminal:"
    echo "  ollama serve"
    echo ""
    echo "Then run this script again."
    exit 1
fi

echo "✅ Ollama is running"
echo ""

# Pull Gemma model
echo "📥 Pulling Gemma 2 (2B) model..."
echo "This may take a few minutes depending on your internet connection..."
echo ""

ollama pull gemma2:2b

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Gemma model downloaded successfully!"
else
    echo ""
    echo "❌ Failed to download Gemma model"
    exit 1
fi

echo ""
echo "=========================================="
echo "Setup Complete! 🎉"
echo "=========================================="
echo ""
echo "You can now:"
echo "1. Run the AI service locally:"
echo "   cd src/Services/AI/AI.API/AI.API"
echo "   dotnet run"
echo ""
echo "2. Or use Docker Compose:"
echo "   docker-compose up -d"
echo "   docker exec -it flightbooking-ollama ollama pull gemma2:2b"
echo ""
echo "3. Access the AI API at: http://localhost:5005/swagger"
echo ""
echo "For more information, see AI_SERVICE_README.md"
echo ""
