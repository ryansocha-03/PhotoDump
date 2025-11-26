# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080

# Copy csproj files for caching
COPY ["server/App.Api/App.Api.csproj", "App.Api/"]
COPY ["server/Core/Core.csproj", "Core/"]
COPY ["server/Infrastructure.EntityFramework/Infrastructure.EntityFramework.csproj", "Infrastructure.EntityFramework/"]
COPY ["server/Identity/Identity.csproj", "Identity/"]

# Restore dependencies
RUN dotnet restore "App.Api/App.Api.csproj"

# Copy all source code
COPY server/ .

# Build and publish
RUN dotnet publish "App.Api/App.Api.csproj" -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "App.Api.dll"]
