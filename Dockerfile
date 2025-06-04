# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Install Node.js and npm
RUN apt-get update && apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs && \
    npm install -g npm@latest

# Copy package.json and package-lock.json first for better caching
COPY package.json ./
RUN npm ci

# Copy csproj and restore as distinct layers
COPY EventMonitoring.ph.csproj ./
RUN dotnet restore

# Copy the rest of the project files
COPY . .

# Build the .NET app
RUN dotnet publish -c Release -o out

# Stage 2: Serve
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
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

ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 
