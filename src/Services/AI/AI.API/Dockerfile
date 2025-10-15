FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/AI/AI.API/AI.API/AI.API.csproj", "src/Services/AI/AI.API/AI.API/"]
COPY ["src/Services/AI/AI.Application/AI.Application/AI.Application.csproj", "src/Services/AI/AI.Application/AI.Application/"]
COPY ["src/Services/AI/AI.Infrastructure/AI.Infrastructure/AI.Infrastructure.csproj", "src/Services/AI/AI.Infrastructure/AI.Infrastructure/"]
COPY ["src/Services/AI/AI.Core/AI.Core/AI.Core.csproj", "src/Services/AI/AI.Core/AI.Core/"]

# Restore dependencies
RUN dotnet restore "src/Services/AI/AI.API/AI.API/AI.API.csproj"

# Copy all source files
COPY . .

# Build
WORKDIR "/src/src/Services/AI/AI.API/AI.API"
RUN dotnet build "AI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AI.API.dll"]
