# Multi-stage build for .NET API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ExpenseTracker.API/ExpenseTracker.API.csproj", "ExpenseTracker.API/"]
COPY ["ExpenseTracker.Dtos/ExpenseTracker.Dtos.csproj", "ExpenseTracker.Dtos/"]
COPY ["ExpenseTracker.Service/ExpenseTracker.Service.csproj", "ExpenseTracker.Service/"]
COPY ["ExpenseTracker.Repository/ExpenseTracker.Repository.csproj", "ExpenseTracker.Repository/"]

RUN dotnet restore "ExpenseTracker.API/ExpenseTracker.API.csproj"

# Copy all source code
COPY . .

# Build and publish the application
WORKDIR "/src/ExpenseTracker.API"
RUN dotnet publish "ExpenseTracker.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/v1/health || exit 1

ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]