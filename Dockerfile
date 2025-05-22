# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY StockAPI/*.csproj ./StockAPI/

# Restore dependencies
RUN dotnet restore

# Copy everything and build
COPY StockAPI/. ./StockAPI/
WORKDIR /app/StockAPI
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/StockAPI/out ./
ENTRYPOINT ["dotnet", "StockAPI.dll"]
