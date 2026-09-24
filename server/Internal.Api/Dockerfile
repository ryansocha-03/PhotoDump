# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080

# Copy csproj files for caching
COPY ["server/Internal.Api/Internal.Api.csproj", "Internal.Api/"]
COPY ["server/ContentStore.MinIO/ContentStore.MinIO.csproj", "MinIO/"]
COPY ["server/Infrastructure.EntityFramework/Infrastructure.EntityFramework.csproj", "Infrastructure.EntityFramework/"]

# Restore dependencies
RUN dotnet restore "Internal.Api/Internal.Api.csproj"

# Copy all source code
COPY server/ .

# Build and publish
RUN dotnet publish "Internal.Api/Internal.Api.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Internal.Api.dll"]

