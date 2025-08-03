# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY EmployeeManagementSystem.Api/*.csproj ./EmployeeManagementSystem.Api/
RUN dotnet restore

COPY . .
WORKDIR /app/EmployeeManagementSystem.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose default port
EXPOSE 80

ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Api.dll"]
