# Stage 1: Node.js Dependencies
FROM node:18 AS node-build

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Stage 2: .NET Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy Node.js dependencies from previous stage
COPY --from=node-build /app/node_modules ./node_modules

# Copy csproj and restore as distinct layers
COPY EventMonitoring.ph.csproj ./
RUN dotnet restore

# Copy the rest of the project files
COPY . .

# Build the .NET application
RUN dotnet build "EventMonitoring.ph.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .
# Copy node_modules
COPY --from=build /app/node_modules ./node_modules
# Copy wwwroot
COPY --from=build /app/wwwroot ./wwwroot

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV NODE_ENV=production

# Expose ports
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 
