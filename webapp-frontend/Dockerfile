# ============================
# Stage 1: Build Angular App
# ============================
FROM node:current-alpine3.22 AS build

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json first (for caching)
COPY package*.json ./

# Install dependencies
RUN npm clean-install

# Copy the rest of the application source
COPY . .

# Build Angular app for production
RUN npm run build -- --configuration production --output-hashing=none

# ============================
# Stage 2: Serve with NGINX
# ============================
FROM nginx:alpine

# Copy built Angular files from Stage 1
COPY --from=build /app/dist/ /usr/share/nginx/html

# Copy custom NGINX config (optional but recommended for SPA routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy env.js to override the existing environment
COPY env.js /usr/share/nginx/html/assets/env.js

# Expose port 80
EXPOSE 80

# Start NGINX
CMD ["nginx", "-g", "daemon off;"]
