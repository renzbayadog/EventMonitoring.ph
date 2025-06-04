# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

<<<<<<< HEAD
# Install Node.js and npm
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g npm@latest

# Copy package.json and package-lock.json first for better caching
COPY package*.json ./
RUN npm ci

=======
# Copy package files
COPY package.json ./

# Install dependencies
RUN npm ci

# Stage 2: .NET Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy Node.js dependencies from previous stage
COPY --from=node-build /app/node_modules ./node_modules

>>>>>>> 178b4211c3f4349afe9612c9f9cbf80c09f429c8
# Copy csproj and restore as distinct layers
COPY EventMonitoring.ph.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the .NET app
RUN dotnet publish -c Release -o out

<<<<<<< HEAD
# Stage 2: Serve
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
=======
# Publish the application
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
>>>>>>> 178b4211c3f4349afe9612c9f9cbf80c09f429c8
WORKDIR /app

# Copy the published output
COPY --from=build /app/out .
# Copy wwwroot folder for static files
COPY --from=build /app/wwwroot ./wwwroot
COPY --from=build /app/node_modules ./node_modules

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

<<<<<<< HEAD
# Set the entrypoint
=======
# Add health check
HEALTHCHECK --interval=30s --timeout=3s \
    CMD curl -f http://localhost/health || exit 1

# Start the application
>>>>>>> 178b4211c3f4349afe9612c9f9cbf80c09f429c8
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 
