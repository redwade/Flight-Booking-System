# Quick Start - AI Service

Get the AI-powered chatbot and recommendations running in 5 minutes!

## Option 1: Local Development (Recommended for Testing)

### Step 1: Install Ollama
```bash
# macOS/Linux
curl -fsSL https://ollama.ai/install.sh | sh

# Or download from https://ollama.ai
```

### Step 2: Run Setup Script
```bash
./setup-ai.sh
```

This will:
- Verify Ollama installation
- Pull the Gemma 2 (2B) model (~1.5GB download)
- Provide next steps

### Step 3: Start the AI Service
```bash
cd src/Services/AI/AI.API/AI.API
dotnet run
```

### Step 4: Test It!
Open http://localhost:5005/swagger and try:

**Chat with AI:**
```json
POST /api/ai/chat
{
  "userId": "test-user",
  "message": "I want to fly from New York to Paris next month"
}
```

**Get Recommendations:**
```json
POST /api/ai/recommendations
{
  "userId": "test-user",
  "origin": "New York",
  "destination": "Paris",
  "departureDate": "2025-11-15",
  "preferredClass": "Economy",
  "maxBudget": 1500
}
```

## Option 2: Docker (Full System)

### Step 1: Start All Services
```bash
docker-compose up -d
```

### Step 2: Pull Gemma Model in Container
```bash
docker exec -it flightbooking-ollama ollama pull gemma2:2b
```

### Step 3: Access Services
- AI API: http://localhost:5005/swagger
- All other services: See main README.md

## Testing the Chatbot

### Simple Conversation
```bash
curl -X POST http://localhost:5005/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "message": "What flights do you have?"
  }'
```

### Multi-turn Conversation
```bash
# First message
curl -X POST http://localhost:5005/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "message": "I want to book a flight to London",
    "sessionId": "my-session"
  }'

# Follow-up (same session)
curl -X POST http://localhost:5005/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "message": "What about business class?",
    "sessionId": "my-session"
  }'
```

## Model Options

### Default: Gemma 2 (2B) - Fast & Lightweight
```bash
ollama pull gemma2:2b  # ~1.5GB, 2GB RAM
```

### Better Quality: Gemma 2 (9B)
```bash
ollama pull gemma2:9b  # ~5GB, 9GB RAM
```

### Best Quality: Gemma 2 (27B)
```bash
ollama pull gemma2:27b  # ~15GB, 27GB RAM
```

To change models, edit `src/Services/AI/AI.Infrastructure/AI.Infrastructure/Services/OllamaAIService.cs`:
```csharp
private readonly string _modelName = "gemma2:9b"; // Change here
```

## Troubleshooting

### "Connection refused" error
```bash
# Make sure Ollama is running
ollama serve

# Check status
curl http://localhost:11434/api/tags
```

### Slow responses
- Use smaller model: `gemma2:2b`
- Enable GPU if available
- Increase timeout in code

### Model not found
```bash
# List installed models
ollama list

# Pull the model
ollama pull gemma2:2b
```

## What's Next?

1. **Integrate with other services**: Connect AI recommendations to Flight Service
2. **Add authentication**: Secure the AI endpoints
3. **Implement RAG**: Connect to your flight database for accurate info
4. **Add streaming**: Real-time chat responses
5. **Monitor usage**: Track AI interactions and performance

## Example Use Cases

### Customer Support Bot
```json
POST /api/ai/chat
{
  "userId": "customer456",
  "message": "How do I change my booking?",
  "sessionId": "support-session-1"
}
```

### Smart Flight Search
```json
POST /api/ai/recommendations
{
  "userId": "traveler789",
  "origin": "San Francisco",
  "destination": "Tokyo",
  "departureDate": "2025-12-01",
  "preferredClass": "Business",
  "maxBudget": 3000
}
```

### Booking Assistant
```json
POST /api/ai/chat
{
  "userId": "user123",
  "message": "Find me the cheapest flight to Miami this weekend",
  "sessionId": "booking-session"
}
```

## Performance Tips

1. **First request is slow**: Model loading takes time (~5-10 seconds)
2. **Subsequent requests are faster**: Model stays in memory
3. **Use GPU**: Significantly faster with NVIDIA GPU
4. **Batch requests**: Process multiple queries efficiently
5. **Cache common queries**: Store frequent responses

## Need Help?

- Full documentation: [AI_SERVICE_README.md](./AI_SERVICE_README.md)
- Ollama docs: https://github.com/ollama/ollama
- Gemma info: https://ai.google.dev/gemma

Happy coding! 🚀
