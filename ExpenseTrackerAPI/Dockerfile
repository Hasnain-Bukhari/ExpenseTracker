# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["ExpenseTracker.sln", "."]

# Copy project files and restore dependencies
COPY ["src/ExpenseTracker.API/ExpenseTracker.API.csproj", "src/ExpenseTracker.API/"]
COPY ["src/ExpenseTracker.Dtos/ExpenseTracker.Dtos.csproj", "src/ExpenseTracker.Dtos/"]
COPY ["src/ExpenseTracker.Service/ExpenseTracker.Service.csproj", "src/ExpenseTracker.Service/"]
COPY ["src/ExpenseTracker.Repository/ExpenseTracker.Repository.csproj", "src/ExpenseTracker.Repository/"]

# Restore dependencies
RUN dotnet restore "ExpenseTracker.sln"

# Copy all source code
COPY src/ ./src/

# Build and publish the application
WORKDIR "/src/src/ExpenseTracker.API"
RUN dotnet publish "ExpenseTracker.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
USER appuser

# Health check (use port 80 internally, Railway maps PORT externally)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/v1/health || exit 1

# Railway sets PORT environment variable automatically
# ASP.NET Core will use it automatically
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]

