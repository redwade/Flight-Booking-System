# Flight Booking System - Tutorials Index

Complete step-by-step tutorials to build a microservices-based flight booking system with AI capabilities.

## 📚 Tutorial Series

### [Part 1: Project Setup & Core Layer](./TUTORIAL_PART1.md)
**Estimated Time**: 2-3 hours | **Difficulty**: Beginner

Learn the fundamentals:
- ✅ Setting up .NET 9.0 microservices project structure
- ✅ Creating domain entities (Booking, Flight, Payment)
- ✅ Implementing repository pattern
- ✅ Understanding clean architecture layers
- ✅ Setting up dependency injection

**What you'll build**: Core domain layer with entities and repository interfaces

---

### [Part 2: Application Layer & CQRS](./TUTORIAL_PART2.md)
**Estimated Time**: 2-3 hours | **Difficulty**: Beginner-Intermediate

Master CQRS pattern:
- ✅ Implementing commands and queries with MediatR
- ✅ Creating command and query handlers
- ✅ Setting up event-driven communication with MassTransit
- ✅ Publishing and consuming events via RabbitMQ
- ✅ Understanding the application layer

**What you'll build**: Application layer with CQRS handlers and event messaging

---

### [Part 3: API Layer & Testing](./TUTORIAL_PART3.md)
**Estimated Time**: 1-2 hours | **Difficulty**: Beginner-Intermediate

Build REST APIs:
- ✅ Creating API controllers with ASP.NET Core
- ✅ Configuring Swagger/OpenAPI documentation
- ✅ Setting up Docker Compose for infrastructure
- ✅ Running and testing the system
- ✅ Making HTTP requests with Swagger UI

**What you'll build**: Complete REST API with Swagger documentation

---

### [Part 4: AI Service with Gemma Integration](./TUTORIAL_PART4_AI_SERVICE.md)
**Estimated Time**: 2-3 hours | **Difficulty**: Intermediate

Add AI-powered features:
- ✅ Installing and configuring Ollama
- ✅ Integrating Google's Gemma AI model
- ✅ Building conversational chatbot with context
- ✅ Creating smart flight recommendations
- ✅ Implementing pattern analysis
- ✅ Advanced topics: RAG, streaming, monitoring

**What you'll build**: AI microservice with chatbot and recommendation engine

---

### [Part 5: API Gateway with Ocelot](./TUTORIAL_PART5_API_GATEWAY.md) ⭐ NEW
**Estimated Time**: 1-2 hours | **Difficulty**: Intermediate

Create a unified entry point:
- ✅ Setting up Ocelot API Gateway
- ✅ Configuring routes for all microservices
- ✅ Implementing rate limiting
- ✅ Adding response caching
- ✅ Load balancing and aggregation
- ✅ Docker integration

**What you'll build**: API Gateway providing single entry point at port 5000

---

## 🎯 Learning Path

### For Complete Beginners
1. Start with **Part 1** - Learn project structure and domain modeling
2. Continue to **Part 2** - Understand CQRS and event-driven architecture
3. Move to **Part 3** - Build REST APIs and test the system
4. Add **Part 4** - AI capabilities (optional but exciting!)
5. Complete with **Part 5** - API Gateway for unified access

### For Experienced Developers
- Jump to **Part 2** if you're familiar with clean architecture
- Skip to **Part 4** if you want to add AI to existing microservices
- Use any part as a reference for specific patterns

### For AI Enthusiasts
- Go directly to **Part 4** for AI integration
- Review **Part 1-3** for microservices context
- Explore advanced AI topics in Part 4

---

## 📊 What You'll Learn

### Architecture & Patterns
- ✅ Microservices architecture
- ✅ Clean architecture (Core, Infrastructure, Application, API)
- ✅ CQRS pattern with MediatR
- ✅ Repository pattern
- ✅ Event-driven architecture
- ✅ Dependency injection

### Technologies
- ✅ .NET 9.0 & ASP.NET Core
- ✅ MediatR for CQRS
- ✅ MassTransit for messaging
- ✅ RabbitMQ for event bus
- ✅ Redis, MongoDB, PostgreSQL databases
- ✅ Ollama & Gemma for AI
- ✅ Docker & Docker Compose
- ✅ Swagger/OpenAPI
- ✅ xUnit & Moq for testing

### Skills
- ✅ Building scalable microservices
- ✅ Implementing clean code principles
- ✅ Writing testable code
- ✅ Integrating AI/LLM models
- ✅ Working with multiple databases
- ✅ Containerization with Docker
- ✅ API design and documentation

---

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- Visual Studio Code / Visual Studio 2022 / Rider
- Ollama (for Part 4)

### Clone and Setup
```bash
# Clone the repository
git clone https://github.com/redwade/Flight-Booking-System.git
cd Flight-Booking-System

# Start infrastructure
docker-compose up -d

# Build the solution
dotnet build

# Run tests
dotnet test
```

### Follow Tutorials
1. Open [TUTORIAL_PART1.md](./TUTORIAL_PART1.md)
2. Follow step-by-step instructions
3. Build and test as you go
4. Move to next part when ready

---

## 📖 Additional Resources

### Documentation
- **[README.md](./README.md)** - Project overview and quick start
- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Detailed architecture documentation
- **[QUICKSTART.md](./QUICKSTART.md)** - Quick reference guide
- **[AI_SERVICE_README.md](./AI_SERVICE_README.md)** - AI service documentation
- **[QUICKSTART_AI.md](./QUICKSTART_AI.md)** - AI quick start guide

### API Examples
- **[API_EXAMPLES.http](./API_EXAMPLES.http)** - HTTP request examples
- **Swagger UI** - Interactive API documentation at each service endpoint

### Setup Scripts
- **[setup.sh](./setup.sh)** - Main system setup
- **[setup-ai.sh](./setup-ai.sh)** - AI service setup
- **[add-packages.sh](./add-packages.sh)** - Package installation

---

## 🎓 Tutorial Features

Each tutorial includes:

### ✅ Step-by-Step Instructions
Clear, numbered steps with explanations

### ✅ Code Examples
Complete, runnable code with comments

### ✅ Explanations
Why we do things, not just how

### ✅ Testing Procedures
How to verify each step works

### ✅ Troubleshooting
Common issues and solutions

### ✅ Best Practices
Industry-standard patterns and conventions

### ✅ Advanced Topics
Going beyond the basics

---

## 💡 Tips for Success

### 1. Take Your Time
- Don't rush through tutorials
- Understand each concept before moving on
- Experiment with the code

### 2. Practice Actively
- Type the code yourself (don't just copy-paste)
- Try variations and modifications
- Break things and fix them

### 3. Use Resources
- Read the documentation links
- Check official .NET docs
- Search for concepts you don't understand

### 4. Test Frequently
- Run tests after each major step
- Use Swagger UI to test APIs
- Verify data in databases

### 5. Ask Questions
- Create GitHub issues for problems
- Review existing issues and discussions
- Share your learnings

---

## 🏆 Completion Checklist

### Part 1 Complete ✅
- [ ] Project structure created
- [ ] Domain entities implemented
- [ ] Repository interfaces defined
- [ ] Understand clean architecture

### Part 2 Complete ✅
- [ ] Commands and queries created
- [ ] MediatR handlers implemented
- [ ] Events published and consumed
- [ ] Understand CQRS pattern

### Part 3 Complete ✅
- [ ] API controllers working
- [ ] Swagger documentation accessible
- [ ] Docker services running
- [ ] Can create and retrieve data

### Part 4 Complete ✅
- [ ] Ollama installed and running
- [ ] Gemma model downloaded
- [ ] AI chatbot responding
- [ ] Recommendations working
- [ ] Understand AI integration

### Part 5 Complete ✅
- [ ] API Gateway project created
- [ ] Ocelot configured
- [ ] All routes working
- [ ] Rate limiting tested
- [ ] Caching verified
- [ ] Understand gateway pattern

---

## 🎯 Next Steps After Tutorials

### Enhance the System
1. Add authentication (JWT) to API Gateway
2. Add distributed tracing (OpenTelemetry)
3. Set up monitoring (Prometheus/Grafana)
4. Implement circuit breaker (Polly)
5. Deploy to cloud (Azure/AWS/GCP)

### Explore Advanced Topics
1. Implement saga pattern
2. Add circuit breaker (Polly)
3. Implement caching strategies
4. Add rate limiting
5. Implement RAG for AI

### Build Your Own
1. Apply patterns to your projects
2. Customize for your use cases
3. Add new features
4. Share your work

---

## 📞 Support & Community

### Get Help
- **GitHub Issues**: Report bugs or ask questions
- **Discussions**: Share ideas and experiences
- **Pull Requests**: Contribute improvements

### Share Your Progress
- Tweet your learnings with #FlightBookingSystem
- Write blog posts about your experience
- Help others in the community

---

## 🌟 What's Next?

After completing all tutorials, you'll have:

✅ A fully functional microservices system  
✅ Deep understanding of clean architecture  
✅ Hands-on experience with CQRS and event-driven design  
✅ AI-powered features in your application  
✅ Production-ready patterns and practices  
✅ Skills to build your own microservices  

**Ready to start?** Begin with [Part 1: Project Setup & Core Layer](./TUTORIAL_PART1.md)

---

**Happy Learning!** 🚀📚

*Last Updated: October 15, 2025*
