# -------------------------------
# STEP 1: Build the application
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY EmployeeManagementSystem.Api/*.csproj ./EmployeeManagementSystem.Api/
COPY EmployeeManagementSystem.Application/*.csproj ./EmployeeManagementSystem.Application/
COPY EmployeeManagementSystem.Domain/*.csproj ./EmployeeManagementSystem.Domain/
COPY EmployeeManagementSystem.Infrastructure/*.csproj ./EmployeeManagementSystem.Infrastructure/

RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/EmployeeManagementSystem.Api
RUN dotnet publish -c Release -o /app/publish

# -------------------------------
# STEP 2: Build runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Set environment variable for ASP.NET to listen on port 80
ENV ASPNETCORE_URLS=http://+:80

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose port 80 (used by Azure App Service)
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Api.dll"]
