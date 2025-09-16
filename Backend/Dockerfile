# ---------------------------------
# STEP 1: Build the application
# ---------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY EmployeeManagementSystem.sln ./
COPY EmployeeManagementSystem.Api/*.csproj ./EmployeeManagementSystem.Api/
COPY EmployeeManagementSystem.Tests/*.csproj ./EmployeeManagementSystem.Tests/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/EmployeeManagementSystem.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


# ---------------------------------
# STEP 2: Runtime image
# ---------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Ensure ASP.NET Core listens on port 80 (required by Azure App Service)
ENV ASPNETCORE_URLS=http://+:80

# Set environment to Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_HOST_PATH=/usr/share/dotnet/dotnet
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

# Create directory for SQLite data (so it survives container restarts if mounted)
RUN mkdir -p /home/data

# Copy published app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Api.dll"]
