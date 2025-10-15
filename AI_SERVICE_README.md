# AI Service - Gemma Integration

The AI Service provides intelligent features to the Flight Booking System using Google's Gemma model via Ollama.

## 🤖 Features

### 1. **AI Chatbot Assistant**
- Conversational AI for customer support
- Context-aware responses with session management
- Helps users with flight searches, bookings, and general inquiries

### 2. **Flight Recommendations**
- Personalized flight recommendations based on user preferences
- Analyzes origin, destination, dates, class preferences, and budget
- Provides intelligent suggestions with reasoning

### 3. **Booking Pattern Analysis**
- Analyzes booking trends and patterns
- Identifies popular destinations and peak times
- Provides insights for business optimization

## 🏗️ Architecture

The AI Service follows the same clean architecture pattern as other microservices:

```
AI/
├── AI.Core/              # Domain entities and interfaces
│   ├── Entities/         # ChatMessage, FlightRecommendation
│   ├── Services/         # IAIService
│   └── Repositories/     # IChatMessageRepository
├── AI.Infrastructure/    # External service implementations
│   ├── Services/         # OllamaAIService (Gemma integration)
│   └── Repositories/     # InMemoryChatMessageRepository
├── AI.Application/       # CQRS handlers
│   ├── Commands/         # SendChatMessageCommand
│   └── Queries/          # GetFlightRecommendationsQuery, GetChatHistoryQuery
└── AI.API/              # REST API endpoints
    └── Controllers/      # AIController
```

## 🚀 Getting Started

### Prerequisites

1. **Install Ollama**: Download from [ollama.ai](https://ollama.ai)
2. **Pull Gemma Model**:
   ```bash
   ollama pull gemma2:2b
   ```
   
   Note: We're using Gemma 2 (2B parameters) as Gemma 3 is not yet available in Ollama. You can also use larger models:
   - `gemma2:9b` - Better quality, requires more resources
   - `gemma2:27b` - Best quality, requires significant GPU memory

### Running Locally

1. **Start Ollama**:
   ```bash
   ollama serve
   ```

2. **Verify Ollama is running**:
   ```bash
   curl http://localhost:11434/api/tags
   ```

3. **Run the AI Service**:
   ```bash
   cd src/Services/AI/AI.API/AI.API
   dotnet run
   ```

4. **Access Swagger UI**: http://localhost:5005/swagger

### Running with Docker

1. **Start all services including Ollama**:
   ```bash
   docker-compose up -d
   ```

2. **Pull the Gemma model inside the Ollama container**:
   ```bash
   docker exec -it flightbooking-ollama ollama pull gemma2:2b
   ```

3. **Access the AI API**: http://localhost:5005/swagger

## 📡 API Endpoints

### 1. Chat with AI Assistant

**POST** `/api/ai/chat`

Send a message to the AI chatbot and get a response.

```json
{
  "userId": "user123",
  "message": "I want to book a flight from New York to London",
  "sessionId": "session-001"  // Optional, for conversation continuity
}
```

**Response:**
```json
{
  "response": "I'd be happy to help you book a flight from New York to London! ...",
  "sessionId": "session-001",
  "timestamp": "2025-10-15T09:30:00Z"
}
```

### 2. Get Flight Recommendations

**POST** `/api/ai/recommendations`

Get AI-powered flight recommendations based on preferences.

```json
{
  "userId": "user123",
  "origin": "New York",
  "destination": "London",
  "departureDate": "2025-11-15",
  "preferredClass": "Business",
  "maxBudget": 2000
}
```

**Response:**
```json
{
  "recommendations": "Based on your preferences, here are my top recommendations:\n\n1. British Airways BA112 - Direct flight...",
  "generatedAt": "2025-10-15T09:30:00Z"
}
```

### 3. Get Chat History

**GET** `/api/ai/chat/history?userId=user123`
**GET** `/api/ai/chat/history?sessionId=session-001`

Retrieve chat history for a user or session.

**Response:**
```json
{
  "messages": [
    {
      "id": "guid",
      "userId": "user123",
      "role": "user",
      "content": "Hello",
      "timestamp": "2025-10-15T09:30:00Z",
      "sessionId": "session-001"
    },
    {
      "id": "guid",
      "userId": "user123",
      "role": "assistant",
      "content": "Hi there! How can I help you today?",
      "timestamp": "2025-10-15T09:30:01Z",
      "sessionId": "session-001"
    }
  ]
}
```

### 4. Health Check

**GET** `/api/ai/health`

Check if the AI service is running.

## 🔧 Configuration

### appsettings.json

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434"
  }
}
```

### Environment Variables

- `Ollama__BaseUrl`: URL of the Ollama service (default: http://localhost:11434)

## 🧪 Testing

Run the AI service tests:

```bash
dotnet test tests/AI.API.Tests/AI.API.Tests/AI.API.Tests.csproj
```

## 💡 Use Cases

### Customer Support Chatbot
```bash
# User asks about flight options
POST /api/ai/chat
{
  "userId": "user123",
  "message": "What flights do you have from Paris to Tokyo in December?",
  "sessionId": "session-001"
}

# Follow-up question
POST /api/ai/chat
{
  "userId": "user123",
  "message": "What about business class options?",
  "sessionId": "session-001"  # Same session for context
}
```

### Personalized Recommendations
```bash
POST /api/ai/recommendations
{
  "userId": "user456",
  "origin": "San Francisco",
  "destination": "Singapore",
  "departureDate": "2025-12-01",
  "preferredClass": "Economy",
  "maxBudget": 1200
}
```

## 🎯 Model Selection

The service uses **Gemma 2 2B** by default for fast responses with reasonable quality. You can change the model in `OllamaAIService.cs`:

```csharp
private readonly string _modelName = "gemma2:2b";
```

Available options:
- `gemma2:2b` - Fast, lightweight (2GB RAM)
- `gemma2:9b` - Better quality (9GB RAM)
- `gemma2:27b` - Best quality (27GB RAM)
- Other Ollama models: `llama3`, `mistral`, `phi3`, etc.

## 🔒 Security Considerations

1. **Rate Limiting**: Consider adding rate limiting to prevent abuse
2. **Input Validation**: Validate and sanitize user inputs
3. **Content Filtering**: Implement content moderation for production
4. **API Keys**: Add authentication for production deployments

## 🚧 Future Enhancements

- [ ] Add streaming responses for real-time chat
- [ ] Implement RAG (Retrieval Augmented Generation) with flight database
- [ ] Add sentiment analysis for customer feedback
- [ ] Implement multi-language support
- [ ] Add voice-to-text integration
- [ ] Cache common queries for faster responses
- [ ] Add analytics and monitoring
- [ ] Implement A/B testing for different models

## 📊 Performance

- **Response Time**: 2-10 seconds depending on model size and query complexity
- **Concurrent Requests**: Supports multiple simultaneous conversations
- **Memory Usage**: 
  - gemma2:2b: ~2GB RAM
  - gemma2:9b: ~9GB RAM
  - gemma2:27b: ~27GB RAM

## 🐛 Troubleshooting

### Ollama not responding
```bash
# Check if Ollama is running
curl http://localhost:11434/api/tags

# Restart Ollama
ollama serve
```

### Model not found
```bash
# Pull the model
ollama pull gemma2:2b

# List available models
ollama list
```

### Slow responses
- Use a smaller model (gemma2:2b)
- Enable GPU acceleration if available
- Increase timeout in `OllamaAIService.cs`

### Docker GPU support
If you have an NVIDIA GPU, ensure:
1. NVIDIA Container Toolkit is installed
2. Docker is configured for GPU support
3. The `deploy` section in docker-compose.yml is uncommented

If you don't have a GPU, remove the `deploy` section from the Ollama service in docker-compose.yml.

## 📚 Resources

- [Ollama Documentation](https://github.com/ollama/ollama)
- [Gemma Model Card](https://ai.google.dev/gemma)
- [Ollama API Reference](https://github.com/ollama/ollama/blob/main/docs/api.md)
