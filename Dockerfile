# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js 16.x
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get update \
    && apt-get install -y nodejs \
    && npm install -g npm@8

# Configure npm to ignore engine requirements
RUN npm config set engine-strict false

# Copy package.json and install frontend dependencies
COPY ["package.json", "package-lock.json", "./"]
RUN npm install --legacy-peer-deps

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

# Copy node_modules from build stage
COPY --from=build /src/node_modules ./wwwroot/node_modules

# Set environment variables
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose port 10000 (Render's default port)
EXPOSE 10000

# Start the application
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 