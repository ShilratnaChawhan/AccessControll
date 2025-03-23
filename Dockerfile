# Pull official Node.js base image
FROM node:20 AS build

# Set the working directory
WORKDIR /app

# Copy package files first to leverage Docker caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the entire application source code
COPY . .

# Build the Angular application
RUN npm run build --prod

# Create a lightweight runtime environment using Node.js
FROM node:20-alpine

# Set the working directory
WORKDIR /app

# Install a minimal HTTP server (Express)
RUN npm install -g serve

# Copy the built Angular app from the previous stage
COPY --from=build /app/dist/access-controll-ui /app

# Expose port 4000
EXPOSE 4000

# Start the Angular app with a Node.js server
CMD ["serve", "-s", "/app", "-l", "4000"]
