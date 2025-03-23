# Pull official base image
FROM node:18 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy package files separately to improve caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the entire application source code
COPY . .

# Build the application
RUN npm run build

# Use a lightweight Nginx image to serve the built application
FROM nginx:1.16.0-alpine

# Remove the default Nginx configuration
RUN rm /etc/nginx/conf.d/default.conf

# Copy the built application from the previous stage
COPY --from=build /app/dist /usr/share/nginx/html

# Copy custom Nginx configuration file
COPY nginx/nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80 to allow external access
EXPOSE 80

# Start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]
