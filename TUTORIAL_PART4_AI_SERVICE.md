# Tutorial Part 4: AI Service with Gemma Integration

Welcome to Part 4 of the Flight Booking System tutorial! In this part, we'll explore the AI Service that provides intelligent features using Google's Gemma model.

## Table of Contents
1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Setting Up the AI Service](#setting-up-the-ai-service)
4. [Understanding the Code](#understanding-the-code)
5. [Building AI Features](#building-ai-features)
6. [Testing the AI Service](#testing-the-ai-service)
7. [Advanced Topics](#advanced-topics)

## Overview

The AI Service adds intelligent capabilities to our flight booking system:
- **Conversational Chatbot**: Natural language support for customer queries
- **Smart Recommendations**: Personalized flight suggestions
- **Pattern Analysis**: Insights into booking behaviors

### Why Gemma?
- Open-source model from Google
- Runs locally via Ollama (no API keys needed)
- Privacy-friendly (data stays on your machine)
- Multiple model sizes for different needs

## Architecture

The AI Service follows the same clean architecture pattern as other microservices:

```
AI Service
├── AI.Core              # Domain Layer
│   ├── Entities         # ChatMessage, FlightRecommendation
│   ├── Services         # IAIService interface
│   └── Repositories     # IChatMessageRepository interface
├── AI.Infrastructure    # Infrastructure Layer
│   ├── Services         # OllamaAIService (Gemma integration)
│   └── Repositories     # InMemoryChatMessageRepository
├── AI.Application       # Application Layer
│   ├── Commands         # SendChatMessageCommand
│   └── Queries          # GetFlightRecommendationsQuery, GetChatHistoryQuery
└── AI.API              # Presentation Layer
    └── Controllers      # AIController
```

### How It Works

```
User Request → API Controller → MediatR → Command/Query Handler → AI Service → Ollama → Gemma Model
                                                                                          ↓
User Response ← API Controller ← MediatR ← Handler ← AI Service ← Ollama ← Gemma Response
```

## Setting Up the AI Service

### Step 1: Install Ollama

**macOS/Linux:**
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

**Windows:**
Download from [https://ollama.ai/download](https://ollama.ai/download)

**Verify Installation:**
```bash
ollama --version
```

### Step 2: Start Ollama

```bash
ollama serve
```

This starts the Ollama server on `http://localhost:11434`

### Step 3: Pull the Gemma Model

```bash
# Lightweight model (recommended for learning)
ollama pull gemma2:2b

# Or use the automated setup script
./setup-ai.sh
```

**Model Options:**
- `gemma2:2b` - 2 billion parameters (~1.5GB, 2GB RAM)
- `gemma2:9b` - 9 billion parameters (~5GB, 9GB RAM)
- `gemma2:27b` - 27 billion parameters (~15GB, 27GB RAM)

### Step 4: Run the AI Service

```bash
cd src/Services/AI/AI.API/AI.API
dotnet run
```

**Access Swagger UI:** http://localhost:5005/swagger

## Understanding the Code

### 1. Core Layer: Domain Entities

**ChatMessage Entity** (`AI.Core/Entities/ChatMessage.cs`):
```csharp
public class ChatMessage
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? SessionId { get; set; }
}
```

**Why this structure?**
- `Role`: Distinguishes between user messages and AI responses
- `SessionId`: Groups related messages for context-aware conversations
- `Timestamp`: Maintains conversation order

### 2. Infrastructure Layer: Ollama Integration

**OllamaAIService** (`AI.Infrastructure/Services/OllamaAIService.cs`):

```csharp
public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _modelName = "gemma2:2b";
    private readonly Dictionary<string, List<Message>> _sessionHistory = new();

    public async Task<string> GenerateChatResponseAsync(
        string userMessage, 
        string? sessionId = null, 
        CancellationToken cancellationToken = default)
    {
        // 1. Get or create session history
        sessionId ??= Guid.NewGuid().ToString();
        
        if (!_sessionHistory.ContainsKey(sessionId))
        {
            _sessionHistory[sessionId] = new List<Message>
            {
                new Message 
                { 
                    Role = "system", 
                    Content = "You are a helpful flight booking assistant..." 
                }
            };
        }

        // 2. Add user message to history
        _sessionHistory[sessionId].Add(new Message 
        { 
            Role = "user", 
            Content = userMessage 
        });

        // 3. Send request to Ollama
        var request = new OllamaRequest
        {
            Model = _modelName,
            Messages = _sessionHistory[sessionId],
            Stream = false
        };

        var response = await _httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken);
        
        // 4. Add AI response to history
        var assistantMessage = result?.Message?.Content ?? "I apologize...";
        _sessionHistory[sessionId].Add(new Message 
        { 
            Role = "assistant", 
            Content = assistantMessage 
        });

        return assistantMessage;
    }
}
```

**Key Concepts:**
- **System Message**: Sets the AI's behavior and personality
- **Session History**: Maintains context across multiple messages
- **Message Roles**: `system`, `user`, `assistant`
- **Context Window**: Keeps last 10 messages to avoid overflow

### 3. Application Layer: CQRS Handlers

**SendChatMessageCommandHandler** (`AI.Application/Commands/SendChatMessageCommandHandler.cs`):

```csharp
public class SendChatMessageCommandHandler : 
    IRequestHandler<SendChatMessageCommand, SendChatMessageResponse>
{
    private readonly IAIService _aiService;
    private readonly IChatMessageRepository _chatMessageRepository;

    public async Task<SendChatMessageResponse> Handle(
        SendChatMessageCommand request, 
        CancellationToken cancellationToken)
    {
        var sessionId = request.SessionId ?? Guid.NewGuid().ToString();

        // Save user message
        await _chatMessageRepository.CreateAsync(new ChatMessage
        {
            UserId = request.UserId,
            Role = "user",
            Content = request.Message,
            SessionId = sessionId
        });

        // Generate AI response
        var aiResponse = await _aiService.GenerateChatResponseAsync(
            request.Message, 
            sessionId, 
            cancellationToken
        );

        // Save assistant message
        await _chatMessageRepository.CreateAsync(new ChatMessage
        {
            UserId = request.UserId,
            Role = "assistant",
            Content = aiResponse,
            SessionId = sessionId
        });

        return new SendChatMessageResponse(aiResponse, sessionId, DateTime.UtcNow);
    }
}
```

**Why CQRS?**
- Separates read and write operations
- Easy to test with mocked dependencies
- Follows single responsibility principle
- Consistent with other microservices

### 4. API Layer: Controllers

**AIController** (`AI.API/Controllers/AIController.cs`):

```csharp
[ApiController]
[Route("api/ai")]
public class AIController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpPost("chat")]
    public async Task<ActionResult<SendChatMessageResponse>> SendChatMessage(
        [FromBody] SendChatMessageRequest request)
    {
        var command = new SendChatMessageCommand(
            request.UserId,
            request.Message,
            request.SessionId
        );

        var response = await _mediator.Send(command);
        return Ok(response);
    }
}
```

## Building AI Features

### Feature 1: Basic Chatbot

**Use Case:** User asks about flights

**Request:**
```json
POST /api/ai/chat
{
  "userId": "user123",
  "message": "What flights do you have from New York to London?"
}
```

**Response:**
```json
{
  "response": "I'd be happy to help you find flights from New York to London! To provide you with the best options, I need a few more details:\n\n1. When would you like to travel?\n2. Do you prefer economy, business, or first class?\n3. Do you have a budget in mind?\n\nOnce I have this information, I can search for the most suitable flights for you.",
  "sessionId": "abc-123-def",
  "timestamp": "2025-10-15T11:30:00Z"
}
```

### Feature 2: Multi-Turn Conversations

**First Message:**
```json
POST /api/ai/chat
{
  "userId": "user123",
  "message": "I want to fly to Paris",
  "sessionId": "session-001"
}
```

**Follow-Up (Same Session):**
```json
POST /api/ai/chat
{
  "userId": "user123",
  "message": "What about business class options?",
  "sessionId": "session-001"  // Same session!
}
```

The AI remembers the context (Paris destination) from the previous message.

### Feature 3: Flight Recommendations

**Use Case:** Get personalized flight suggestions

**Request:**
```json
POST /api/ai/recommendations
{
  "userId": "user123",
  "origin": "San Francisco",
  "destination": "Tokyo",
  "departureDate": "2025-12-01",
  "preferredClass": "Business",
  "maxBudget": 3000
}
```

**Response:**
```json
{
  "recommendations": "Based on your preferences for a business class flight from San Francisco to Tokyo with a budget of $3000, here are my top recommendations:\n\n1. **Direct Flight Option**\n   - Airline: United Airlines or ANA\n   - Flight time: ~11 hours\n   - Why: Direct flights save time and are more convenient\n   - Expected price: $2,500-$2,800\n\n2. **Premium Economy Alternative**\n   - Consider premium economy if business class exceeds budget\n   - More legroom and better service than economy\n   - Price: $1,500-$1,800\n\n3. **Timing Tip**\n   - Book 2-3 months in advance for best prices\n   - Tuesday/Wednesday departures often cheaper\n   - Avoid peak holiday season (Dec 20-Jan 5)\n\n4. **Airline Recommendations**\n   - ANA: Excellent service, Japanese hospitality\n   - United: Good connections, Star Alliance benefits\n   - JAL: Premium experience, great food\n\nWould you like me to help you search for specific flights?",
  "generatedAt": "2025-10-15T11:30:00Z"
}
```

### Feature 4: Chat History

**Retrieve Conversation History:**
```bash
GET /api/ai/chat/history?sessionId=session-001
```

**Response:**
```json
{
  "messages": [
    {
      "id": "guid-1",
      "userId": "user123",
      "role": "user",
      "content": "I want to fly to Paris",
      "timestamp": "2025-10-15T11:25:00Z",
      "sessionId": "session-001"
    },
    {
      "id": "guid-2",
      "userId": "user123",
      "role": "assistant",
      "content": "Great! I can help you find flights to Paris...",
      "timestamp": "2025-10-15T11:25:05Z",
      "sessionId": "session-001"
    }
  ]
}
```

## Testing the AI Service

### Unit Tests

**Testing the Controller** (`AI.API.Tests/AIControllerTests.cs`):

```csharp
[Fact]
public async Task SendChatMessage_ReturnsOkResult_WithResponse()
{
    // Arrange
    var request = new SendChatMessageRequest("user123", "Hello", "session-001");
    var expectedResponse = new SendChatMessageResponse("Hi there!", "session-001", DateTime.UtcNow);

    _mediatorMock
        .Setup(m => m.Send(It.IsAny<SendChatMessageCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

    // Act
    var result = await _controller.SendChatMessage(request);

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var response = Assert.IsType<SendChatMessageResponse>(okResult.Value);
    Assert.Equal(expectedResponse.Response, response.Response);
}
```

**Run Tests:**
```bash
dotnet test tests/AI.API.Tests/AI.API.Tests/AI.API.Tests.csproj
```

### Manual Testing with curl

**Test Chat:**
```bash
curl -X POST http://localhost:5005/api/ai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user",
    "message": "Hello, I need help booking a flight"
  }'
```

**Test Recommendations:**
```bash
curl -X POST http://localhost:5005/api/ai/recommendations \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "test-user",
    "origin": "New York",
    "destination": "London",
    "departureDate": "2025-11-15",
    "preferredClass": "Economy",
    "maxBudget": 1000
  }'
```

### Integration Testing

**Test with Swagger UI:**
1. Open http://localhost:5005/swagger
2. Expand `/api/ai/chat` endpoint
3. Click "Try it out"
4. Enter test data
5. Click "Execute"
6. View the AI response

## Advanced Topics

### 1. Customizing the AI Personality

Edit the system message in `OllamaAIService.cs`:

```csharp
new Message 
{ 
    Role = "system", 
    Content = @"You are a professional flight booking assistant named 'SkyBot'. 
                You are friendly, efficient, and knowledgeable about air travel.
                Always provide accurate information and ask clarifying questions.
                Keep responses concise but helpful.
                Use bullet points for multiple options." 
}
```

### 2. Switching Models

**Change to a larger model for better quality:**

```csharp
// In OllamaAIService.cs
private readonly string _modelName = "gemma2:9b"; // Instead of gemma2:2b
```

**Pull the new model:**
```bash
ollama pull gemma2:9b
```

### 3. Adding Streaming Responses

For real-time chat experience:

```csharp
public async IAsyncEnumerable<string> GenerateChatResponseStreamAsync(
    string userMessage, 
    string? sessionId = null)
{
    var request = new OllamaRequest
    {
        Model = _modelName,
        Messages = _sessionHistory[sessionId],
        Stream = true  // Enable streaming
    };

    using var response = await _httpClient.PostAsStreamAsync("/api/chat", request);
    using var reader = new StreamReader(response);
    
    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();
        if (!string.IsNullOrEmpty(line))
        {
            var chunk = JsonSerializer.Deserialize<OllamaStreamResponse>(line);
            yield return chunk?.Message?.Content ?? "";
        }
    }
}
```

### 4. Implementing RAG (Retrieval Augmented Generation)

Connect AI to your flight database for accurate information:

```csharp
public async Task<string> GenerateFlightRecommendationAsync(
    string userPreferences, 
    CancellationToken cancellationToken = default)
{
    // 1. Query flight database based on preferences
    var availableFlights = await _flightRepository.SearchAsync(preferences);
    
    // 2. Format flight data for AI
    var flightContext = FormatFlightsForAI(availableFlights);
    
    // 3. Create prompt with real data
    var prompt = $@"Based on these available flights:
{flightContext}

And user preferences:
{userPreferences}

Recommend the best options and explain why.";

    // 4. Get AI recommendation
    return await GenerateChatResponseAsync(prompt);
}
```

### 5. Adding Conversation Memory

**Persistent Storage with Redis:**

```csharp
public class RedisChatMessageRepository : IChatMessageRepository
{
    private readonly IConnectionMultiplexer _redis;

    public async Task<ChatMessage> CreateAsync(ChatMessage message)
    {
        var db = _redis.GetDatabase();
        var key = $"chat:{message.SessionId}:{message.Id}";
        var json = JsonSerializer.Serialize(message);
        await db.StringSetAsync(key, json);
        return message;
    }

    public async Task<IEnumerable<ChatMessage>> GetBySessionIdAsync(string sessionId)
    {
        var db = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"chat:{sessionId}:*");
        
        var messages = new List<ChatMessage>();
        foreach (var key in keys)
        {
            var json = await db.StringGetAsync(key);
            var message = JsonSerializer.Deserialize<ChatMessage>(json);
            if (message != null) messages.Add(message);
        }
        
        return messages.OrderBy(m => m.Timestamp);
    }
}
```

### 6. Rate Limiting

Protect your AI service from abuse:

```csharp
// In Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ai-chat", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10; // 10 requests per minute
    });
});

// In AIController.cs
[EnableRateLimiting("ai-chat")]
[HttpPost("chat")]
public async Task<ActionResult<SendChatMessageResponse>> SendChatMessage(...)
```

### 7. Monitoring and Analytics

Track AI usage:

```csharp
public class AIAnalyticsService
{
    public async Task TrackChatAsync(string userId, string message, string response)
    {
        await _analyticsRepository.CreateAsync(new AIAnalytics
        {
            UserId = userId,
            MessageLength = message.Length,
            ResponseLength = response.Length,
            ResponseTime = stopwatch.ElapsedMilliseconds,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

## Docker Deployment

### docker-compose.yml Configuration

```yaml
# Ollama service
ollama:
  image: ollama/ollama:latest
  container_name: flightbooking-ollama
  ports:
    - "11434:11434"
  volumes:
    - ollama_data:/root/.ollama
  networks:
    - flightbooking-network

# AI API service
ai-api:
  build:
    context: .
    dockerfile: src/Services/AI/AI.API/Dockerfile
  container_name: ai-api
  ports:
    - "5005:8080"
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - Ollama__BaseUrl=http://ollama:11434
  depends_on:
    - ollama
  networks:
    - flightbooking-network
```

### Deploy with Docker

```bash
# 1. Build and start services
docker-compose up -d

# 2. Pull Gemma model in container
docker exec -it flightbooking-ollama ollama pull gemma2:2b

# 3. Verify AI service
curl http://localhost:5005/api/ai/health
```

## Best Practices

### 1. Error Handling
```csharp
try
{
    var response = await _aiService.GenerateChatResponseAsync(message);
    return Ok(response);
}
catch (HttpRequestException ex)
{
    _logger.LogError(ex, "Ollama service unavailable");
    return StatusCode(503, "AI service temporarily unavailable");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error generating AI response");
    return StatusCode(500, "An error occurred");
}
```

### 2. Timeout Configuration
```csharp
services.AddHttpClient<IAIService, OllamaAIService>(client =>
{
    client.BaseAddress = new Uri(ollamaBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(2); // Generous timeout for LLM
});
```

### 3. Input Validation
```csharp
[HttpPost("chat")]
public async Task<ActionResult<SendChatMessageResponse>> SendChatMessage(
    [FromBody] SendChatMessageRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Message))
        return BadRequest("Message cannot be empty");
    
    if (request.Message.Length > 1000)
        return BadRequest("Message too long (max 1000 characters)");
    
    // Process request...
}
```

### 4. Logging
```csharp
_logger.LogInformation(
    "Chat request from user {UserId}, session {SessionId}, message length {Length}",
    request.UserId,
    request.SessionId,
    request.Message.Length
);
```

## Troubleshooting

### Common Issues

**1. "Connection refused" error**
```bash
# Check if Ollama is running
curl http://localhost:11434/api/tags

# If not, start it
ollama serve
```

**2. "Model not found" error**
```bash
# List installed models
ollama list

# Pull the required model
ollama pull gemma2:2b
```

**3. Slow responses**
- Use smaller model: `gemma2:2b`
- Enable GPU if available
- Reduce context window size
- Implement caching for common queries

**4. Out of memory**
- Use smaller model
- Reduce batch size
- Close other applications
- Increase system swap space

## Summary

In this tutorial, you learned:

✅ How to integrate Gemma AI model using Ollama  
✅ Clean architecture for AI services  
✅ CQRS pattern with MediatR  
✅ Building conversational chatbots  
✅ Generating personalized recommendations  
✅ Testing AI features  
✅ Advanced topics (RAG, streaming, monitoring)  
✅ Docker deployment  
✅ Best practices and troubleshooting  

## Next Steps

1. **Experiment**: Try different prompts and models
2. **Integrate**: Connect AI to Flight/Booking services
3. **Enhance**: Add RAG with real flight data
4. **Deploy**: Set up production environment
5. **Monitor**: Track usage and performance

## Additional Resources

- [Ollama Documentation](https://github.com/ollama/ollama)
- [Gemma Model Card](https://ai.google.dev/gemma)
- [AI_SERVICE_README.md](./AI_SERVICE_README.md) - Detailed service docs
- [QUICKSTART_AI.md](./QUICKSTART_AI.md) - Quick start guide

---

**Previous Tutorial**: [Part 3: API Layer & Testing](./TUTORIAL_PART3.md)  
**Next Tutorial**: [Part 5: API Gateway with Ocelot](./TUTORIAL_PART5_API_GATEWAY.md)

Happy coding! 🚀🤖
