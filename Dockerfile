# Stage 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY EmployeeManagement/*.csproj ./EmployeeManagement/
RUN dotnet restore ./EmployeeManagement/EmployeeManagement.csproj

# Copy the rest of the source code
COPY EmployeeManagement/. ./EmployeeManagement/

# Publish the app
RUN dotnet publish ./EmployeeManagement/EmployeeManagement.csproj -c Release -o /app

# Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "EmployeeManagement.dll"]
