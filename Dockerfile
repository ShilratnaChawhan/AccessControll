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
RUN npm run build 

# Create a lightweight runtime environment using Node.js
FROM node:20-alpine

# Set the working directory
WORKDIR /app

# Install Express.js
RUN npm install express

# Copy the built Angular app from the previous stage
COPY --from=build /app/dist/access-controll-ui /app

# Copy the custom Express server script
COPY server.js .

# Expose port 3000
EXPOSE 3000

# Start the Angular app with Express.js
CMD ["node", "server.js"]
