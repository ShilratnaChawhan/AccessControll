# Step 1: Build Angular Frontend
FROM node:18 AS frontend-build
WORKDIR /app
COPY AccessControll-UI/ ./AccessControll-UI/
WORKDIR /app/AccessControll-UI
RUN npm install
RUN npm run build -- --output-path=dist

# Step 2: Build .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY AccessControll-API/ ./AccessControll-API/
WORKDIR /app/AccessControll-API
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Step 3: Create Final Image with .NET 8 Runtime and Nginx
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Install Nginx
RUN apk add --no-cache nginx

# Set up working directory
WORKDIR /app

# Copy backend build
COPY --from=backend-build /app/AccessControll-API/out /app

# Copy frontend build to Nginx html directory
COPY --from=frontend-build /app/AccessControll-UI/dist /usr/share/nginx/html

# Copy Nginx config
COPY nginx/nginx.conf /etc/nginx/nginx.conf

# Copy entrypoint script
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Start both .NET API and Nginx
CMD ["/bin/sh", "/entrypoint.sh"]
