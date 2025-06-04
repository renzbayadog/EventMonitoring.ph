# Use the official .NET SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set working directory
WORKDIR /app

# Install Node.js
RUN apt-get update && \
    apt-get install -y nodejs npm && \
    npm install -g npm@latest

# Copy package.json and package-lock.json
COPY package*.json ./

# Install Node.js dependencies
RUN npm install

# Copy the rest of the project files
COPY . .

# Build the .NET application
RUN dotnet build "EventMonitoring.ph.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "EventMonitoring.ph.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/node_modules ./node_modules
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "EventMonitoring.ph.dll"] 