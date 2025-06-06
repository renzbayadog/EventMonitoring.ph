# Stage 1: Node.js Dependencies
FROM node:18-alpine AS node-build

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci --prefer-offline --no-audit && \
    npm dedupe && \
    npm prune --production

# Stage 2: .NET Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy Node.js dependencies from previous stage
COPY --from=node-build /app/node_modules ./node_modules

# Copy csproj and restore as distinct layers
COPY EventMonitoring.ph.csproj ./
RUN dotnet restore --no-cache --force -r linux-x64 && \
    dotnet tool restore

# Copy the rest of the project files
COPY . .

# Build the .NET application
RUN dotnet build "EventMonitoring.ph.csproj" -c Release -o /app/build -r linux-x64

# Publish the application
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish \
    --no-restore \
    --no-self-contained \
    -r linux-x64 \
    /p:UseSharedCompilation=false \
    /p:BuildInParallel=false \
    /m:1 \
    /p:RuntimeIdentifier=linux-x64

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set working directory
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .
# Copy node_modules
COPY --from=node-build /app/node_modules ./node_modules
# Copy wwwroot
COPY --from=build /app/wwwroot ./wwwroot

# Set environment variables
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV NODE_ENV=production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Expose port for Render
EXPOSE 10000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl --fail http://localhost:10000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 