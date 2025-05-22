# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy everything
COPY . .

# Build and publish
RUN dotnet publish -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "StockAPI.dll"]
