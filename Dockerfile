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

# Step 3: Create Final Image with Nginx and .NET Runtime
FROM nginx:1.16.0-alpine AS final

# Set up working directory
WORKDIR /app

# Copy frontend build to Nginx html directory
COPY --from=frontend-build /app/AccessControll-UI/dist/* /usr/share/nginx/html

# Copy backend build to the same container
COPY --from=backend-build /app/AccessControll-API/out /app

# Copy custom Nginx config
COPY nginx/nginx.conf /etc/nginx/nginx.conf

# Copy entrypoint script
COPY AccessControll-API.sln /AccessControll-API.sln
RUN chmod +x /AccessControll-API.sln

# Start both services
CMD ["/AccessControll-API.sln"]
