# Step 1: Build the Angular app using Node.js
FROM node:20 AS build

# Set the working directory
WORKDIR /app

# Copy package files separately for better caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the entire application source code
COPY . .

# Build the Angular application
RUN npm run build --prod

# Step 2: Serve the Angular app using Nginx
FROM nginx:alpine

# Copy the built Angular app to the Nginx server directory
COPY --from=build /app/dist/access-controll-ui /usr/share/nginx/html

# Copy the custom Nginx configuration (if needed)
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 80 for HTTP traffic
EXPOSE 80

# Start Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]
