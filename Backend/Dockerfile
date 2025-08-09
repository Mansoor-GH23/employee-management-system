# -------------------------------
# STEP 1: Build the application
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY EmployeeManagementSystem.sln .
COPY EmployeeManagementSystem.Api/*.csproj ./EmployeeManagementSystem.Api/
COPY EmployeeManagementSystem.Tests/*.csproj ./EmployeeManagementSystem.Tests/

RUN dotnet restore

COPY . .

WORKDIR /app/EmployeeManagementSystem.Api
RUN dotnet publish -c Release -o /app/publish

# -------------------------------
# STEP 2: Runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80

# Create persistent directory for SQLite DB
RUN mkdir -p /home/data

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "EmployeeManagementSystem.Api.dll"]
