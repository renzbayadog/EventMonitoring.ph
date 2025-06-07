# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["EventMonitoring.ph.csproj", "./"]
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet build "EventMonitoring.ph.csproj" -c Release -o /app/build
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 