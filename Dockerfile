# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set working directory
WORKDIR /app

# Install Node.js and npm
RUN apt-get update && \
    apt-get install -y nodejs npm && \
    npm install -g npm@latest

# Copy package.json and package-lock.json first for better caching
COPY package*.json ./

# Install Node.js dependencies
RUN npm ci --only=production

# Copy csproj and restore as distinct layers
COPY EventMonitoring.ph.csproj ./
RUN dotnet restore

# Copy the rest of the project files
COPY . .

# Build the .NET application
RUN dotnet build "EventMonitoring.ph.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .
# Copy node_modules from build stage
COPY --from=build /app/node_modules ./node_modules
# Copy wwwroot folder for static files
COPY --from=build /app/wwwroot ./wwwroot

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV NODE_ENV=production

# Start the application
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 