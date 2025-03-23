# Pull official Node.js base image
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

# Use a lightweight Node.js image for serving the app
FROM node:20-alpine

# Set the working directory
WORKDIR /app

# Install 'serve' globally
RUN npm install -g serve

# Copy the built Angular app from the previous stage
COPY --from=build /app/dist/access-controll-ui /app

# Expose port 3000
EXPOSE 3000

# Start the Angular app using 'serve'
CMD ["serve", "-s", "/app", "-l", "3000"]
